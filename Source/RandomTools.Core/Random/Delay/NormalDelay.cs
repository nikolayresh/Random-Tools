using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates delays using a truncated normal (Gaussian) distribution.
	/// Values always fall within [Minimum, Maximum].
	/// </summary>
	public sealed class NormalDelay : RandomDelay<DelayOptions.Normal>
	{
		/// <summary>
		/// Provides statistical helpers for determining feasibility and calculating
		/// CDF for a normal distribution.
		/// </summary>
		private static class GaussianTools
		{
			/// <summary>
			/// Returns true if the normal distribution N(mean, stdDev) assigns
			/// ANY probability mass within [min, max].
			/// </summary>
			public static bool RangeHasMass(double mean, double stdDev, (double Min, double Max) range)
			{
				double zMin = (range.Min - mean) / stdDev;
				double zMax = (range.Max - mean) / stdDev;

				// Total mass inside the interval
				double probability = CDF(zMax) - CDF(zMin);
				return probability > 0.0;
			}

			/// <summary>
			/// Standard Normal Cumulative Distribution Function (CDF).
			/// </summary>
			public static double CDF(double z)
			{
				const double a1 = 0.254829592;
				const double a2 = -0.284496736;
				const double a3 = 1.421413741;
				const double a4 = -1.453152027;
				const double a5 = 1.061405429;
				const double p = 0.3275911;

				double sign = z < 0 ? -1 : 1;
				z = Math.Abs(z) / Math.Sqrt(2.0);

				double t = 1.0 / (1.0 + p * z);
				double y =
					1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1)
					* t * Math.Exp(-z * z);

				return 0.5 * (1.0 + sign * y);
			}

			/// <summary>
			/// Generates a single normally distributed value via the Box-Muller transform.
			/// Caches the second generated value for subsequent calls.
			/// </summary>
			public static double NextValue(double mean, double stdDev, ref double? cache)
			{
				// Handle case when Standard Deviation is 0
				if (stdDev <= double.Epsilon)
					return mean;

				// Check for the cached value (Z2 from the previous call)
				if (cache != null)
				{
					double inCache = cache.Value;
					cache = null;

					// Return the cached value
					return mean + (inCache * stdDev);
				}

				// Generate two uniformly random values via Box-Muller method.
				// U1 must never be exactly zero to avoid log(0). 
				// By using (1.0 - NextDouble()), since NextDouble() returns [0.0, 1.0),
				// (1.0 - NextDouble()) returns (0.0, 1.0].
				double u1 = 1.0 - CoreTools.NextDouble();
				double u2 = CoreTools.NextDouble();

				// Box-Muller formulas
				double radius = Math.Sqrt(-2.0 * Math.Log(u1));
				double angle = 2.0 * Math.PI * u2;

				// Z1 (cosine) is the value to return
				double z1 = radius * Math.Cos(angle);

				// Z2 (sine) is the value to cache
				double z2 = radius * Math.Sin(angle);

				// Cache Z2
				cache = z2;

				// Return Z1 (scaled and shifted)
				return mean + (z1 * stdDev);
			}
		}

		/// <summary>
		/// The maximum number of rejection attempts allowed when sampling
		/// a value inside the allowed range.
		/// </summary>
		private const int ResampleAttempts = 1_000_000;

#pragma warning disable IDE0290 // Use primary constructor
		public NormalDelay(DelayOptions.Normal options) : base(options) { }
#pragma warning restore IDE0290 // Use primary constructor

		/// <summary>
		/// Produces a delay drawn from a normal distribution and clamped
		/// to [Minimum, Maximum] via truncated rejection sampling.
		/// </summary>
		public override TimeSpan Next()
		{
			double mean = Options.Mean;
			double stdDev = Options.StandardDeviation;

			// Handle case when Standard Deviation is 0
			if (stdDev <= double.Epsilon)
			{
				// Check if the single-point mean is within the allowed range
				if (Options.Minimum <= mean && mean <= Options.Maximum)
				{
					// include mean if it's located within the range
					return CoreTools.ToTimeSpan(mean, Options.TimeUnit);
				}

				// If stdDev is 0, the distribution is a single point at 'mean'.
				// If 'mean' is outside the range [Minimum, Maximum], sampling is impossible.
				throw new NextGeneratorException(Options,
					$"The normal distribution (mean={mean}, stdDev={stdDev}) has zero probability " +
					$"inside the range [{Options.Minimum}, {Options.Maximum}]. Sampling is impossible.");
			}

			// Check if sampling inside the provided range is even possible
			if (!GaussianTools.RangeHasMass(mean, stdDev, (Options.Minimum, Options.Maximum)))
			{
				throw new NextGeneratorException(Options,
					$"The normal distribution (mean={mean}, stdDev={stdDev}) has zero probability " +
					$"inside the range [{Options.Minimum}, {Options.Maximum}]. Sampling is impossible.");
			}

			double? cache = null;
			int attempts = ResampleAttempts;

			while (true)
			{
				double value = GaussianTools.NextValue(mean, stdDev, ref cache);

				// the generated value did not hit the provided range
				if (value < Options.Minimum || value > Options.Maximum)
				{
					if (attempts-- != 0)
						continue;

					// Extremely improbable, but cannot allow an infinite loop
					throw new NextGeneratorException(Options,
						$"Failed to generate a value within [{Options.Minimum}, {Options.Maximum}] after {ResampleAttempts} attempts. " +
						 "A different distribution configuration may be required.");
				}

				return CoreTools.ToTimeSpan(value, Options.TimeUnit);
			}
		}
	}
}
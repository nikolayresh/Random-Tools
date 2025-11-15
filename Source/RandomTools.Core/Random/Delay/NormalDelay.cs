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
				double probability = CumulativeDistributionFunction(zMax) - CumulativeDistributionFunction(zMin);
				return probability > 0.0;
			}

			/// <summary>
			/// Standard Normal Cumulative Distribution Function (CDF).
			/// </summary>
			public static double CumulativeDistributionFunction(double z)
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
		}

		/// <summary>
		/// The maximum number of rejection attempts allowed when sampling
		/// a value inside the allowed range.
		/// </summary>
		private const int MaxRejectionAttempts = 5_000_000;

#pragma warning disable IDE0290 // Use primary constructor
		public NormalDelay(DelayOptions.Normal options) : base(options) { }
#pragma warning restore IDE0290 // Use primary constructor

		/// <summary>
		/// Generates a single normally distributed value via the Box-Muller transform.
		/// </summary>
		private static double NextGaussianValue(double mean, double stdDev)
		{
			// U1 must never be exactly zero to avoid log(0)
			double u1 = 1.0 - CoreTools.NextDouble();
			double u2 = CoreTools.NextDouble();

			double radius = Math.Sqrt(-2.0 * Math.Log(u1));
			double angle = 2.0 * Math.PI * u2;

			return mean + (radius * Math.Cos(angle) * stdDev);
		}

		/// <summary>
		/// Produces a delay drawn from a normal distribution and clamped
		/// to [Minimum, Maximum] via truncated rejection sampling.
		/// </summary>
		public override TimeSpan Next()
		{
			double mean = Options.Mean;
			double stdDev = Options.StandardDeviation;

			// Check if sampling inside the provided range is even possible
			if (!GaussianTools.RangeHasMass(mean, stdDev, (Options.Minimum, Options.Maximum)))
			{
				throw new NextGeneratorException(Options,
					$"The normal distribution (mean={mean}, stdDev={stdDev}) has zero probability " +
					$"inside the range [{Options.Minimum}, {Options.Maximum}]. Sampling is impossible.");
			}

			int attempts = MaxRejectionAttempts;

			while (true)
			{
				double value = NextGaussianValue(mean, stdDev);

				if (value < Options.Minimum || value > Options.Maximum)
				{
					if (attempts-- != 0)
						continue;

					// Extremely improbable, but cannot allow an infinite loop
					throw new NextGeneratorException(Options,
						$"Failed to generate a value within [{Options.Minimum}, {Options.Maximum}] after {MaxRejectionAttempts} attempts. " +
						"A different distribution configuration may be required.");
				}

				return CoreTools.ToTimeSpan(value, Options.TimeUnit);
			}
		}
	}
}
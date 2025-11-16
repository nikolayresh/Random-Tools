namespace RandomTools.Core
{
	/// <summary>
	/// Utilities for the Normal (Gaussian) distribution.
	/// Includes:
	/// - Probability calculations for intervals,
	/// - Minimum attempts calculation for success probabilities,
	/// - Random value generation using the Box-Muller transform with caching.
	/// </summary>
	public static class GaussianTools
	{
		/// <summary>
		/// Computes the probability that a normal distribution N(mean, stdDev²) 
		/// produces a value within the interval [Min, Max].
		/// </summary>
		/// <param name="mean">Mean (μ) of the distribution.</param>
		/// <param name="stdDev">Standard deviation (σ) of the distribution.</param>
		/// <param name="range">Interval to evaluate (Min, Max).</param>
		/// <returns>Probability of a value falling in the interval.</returns>
		public static double GetRangeHitRate(double mean, double stdDev, (double Min, double Max) range)
		{
			if (range.Min >= range.Max)
				return Probability.Impossible;

			if (stdDev <= double.Epsilon)
			{
				return (range.Min <= mean && mean <= range.Max)
					? Probability.Certain
					: Probability.Impossible;
			}

			double zMin = (range.Min - mean) / stdDev;
			double zMax = (range.Max - mean) / stdDev;

			return NormalCDF(zMax) - NormalCDF(zMin);
		}

		/// <summary>
		/// Standard normal cumulative distribution function (CDF) approximation.
		/// </summary>
		/// <param name="z">Z-score.</param>
		/// <returns>Cumulative probability P(Z ≤ z) for Z ~ N(0,1).</returns>
		private static double NormalCDF(double z)
		{
			const double a1 = 0.254829592, a2 = -0.284496736, a3 = 1.421413741;
			const double a4 = -1.453152027, a5 = 1.061405429, p = 0.3275911;

			double sign = Math.Sign(z);
			double x = Math.Abs(z) / Math.Sqrt(2.0);

			double t = 1.0 / (1.0 + p * x);
			double y = 1.0 - (((((a5 * t + a4) * t + a3) * t + a2) * t + a1) * t) * Math.Exp(-x * x);

			return 0.5 * (1.0 + sign * y);
		}

		/// <summary>
		/// Determines the minimum number of attempts needed to achieve at least one success
		/// given a per-attempt success probability (<paramref name="hitRate"/>),
		/// ensuring that the probability of total failure does not exceed <paramref name="epsilon"/>.
		/// </summary>
		/// <param name="hitRate">Probability of success for a single attempt (0 ≤ hitRate ≤ 1).</param>
		/// <param name="epsilon">
		/// Maximum acceptable probability of failure across all attempts.
		/// Must be in the range (0, 1). Default is 1E-6.
		/// </param>
		/// <returns>
		/// Minimum number of attempts required to achieve at least one success with the specified confidence.
		/// Returns <see cref="int.MaxValue"/> if success is practically impossible (hitRate ≈ 0),
		/// or 1 if success is essentially guaranteed (hitRate ≈ 1).
		/// </returns>
		public static int GetHitAttempts(double hitRate, double epsilon = 1E-6)
		{
			if (hitRate < 0.0 || hitRate > 1.0)
				throw new ArgumentOutOfRangeException(nameof(hitRate), "Hit rate must be between 0 and 1.");
			if (epsilon <= 0.0 || epsilon >= 1.0)
				throw new ArgumentOutOfRangeException(nameof(epsilon), "Epsilon must be in (0, 1).");

			if (hitRate <= double.Epsilon)
				return int.MaxValue;

			if ((1.0 - hitRate) <= double.Epsilon)
				return 1;

			double attempts = Math.Ceiling(Math.Log(epsilon) / Math.Log(1.0 - hitRate));

			if (attempts > int.MaxValue)
			{
				throw new ArgumentException(
					$"Required attempts ({attempts:N0}) exceed int.MaxValue.",
					nameof(epsilon));
			}

			return Math.Max((int)attempts, 1);
		}

		/// <summary>
		/// Generates a normally distributed random value with the specified mean and standard deviation.
		/// Uses the Box-Muller transform to convert uniform random numbers to standard normal.
		/// </summary>
		/// <param name="mean">Mean (μ) of the distribution.</param>
		/// <param name="stdDev">Standard deviation (σ).</param>
		/// <param name="cache">Optional cached Gaussian value from a previous call.</param>
		/// <returns>Random value drawn from N(mean, σ²).</returns>
		public static double NextValue(double mean, double stdDev, ref double? cache)
		{
			if (stdDev <= double.Epsilon)
				return mean;

			if (cache != null)
			{
				double inCache = cache.Value;
				cache = null;

				return mean + (inCache * stdDev);
			}

			// Generate two independent standard normal values
			double u1 = 1.0 - CoreTools.NextDouble(); // avoid Math.Log(0)
			double u2 = CoreTools.NextDouble();

			double radius = Math.Sqrt(-2.0 * Math.Log(u1));
			double angle = 2.0 * Math.PI * u2;

			double z1 = radius * Math.Cos(angle);
			double z2 = radius * Math.Sin(angle);

			cache = z2; // store second value for next call
			return mean + (z1 * stdDev);
		}
	}
}

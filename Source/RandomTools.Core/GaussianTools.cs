namespace RandomTools.Core
{
	/// <summary>
	/// Provides utility methods for working with the Normal (Gaussian) distribution.
	/// Includes:
	/// - Calculating probabilities that a value falls within a specific range,
	/// - Computing the minimum number of attempts to achieve a success with high confidence,
	/// - Generating random values using a normal distribution via the Box-Muller transform.
	/// </summary>
	internal static class GaussianTools
	{
		/// <summary>
		/// Computes the probability that a normally distributed variable with mean <paramref name="mean"/> 
		/// and standard deviation <paramref name="stdDev"/> falls within a specified range [Min, Max].
		/// </summary>
		/// <param name="mean">Mean (μ) of the normal distribution.</param>
		/// <param name="stdDev">Standard deviation (σ) of the distribution. Must be non-negative.</param>
		/// <param name="range">Tuple containing the minimum and maximum bounds of the interval.</param>
		/// <returns>
		/// Probability that a value sampled from N(mean, σ²) lies within [Min, Max].
		/// Returns 0 if the interval is invalid or the standard deviation is zero and the mean is outside the range.
		/// Returns 1 if the standard deviation is zero and the mean is inside the range.
		/// </returns>
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
		/// Approximates the cumulative distribution function (CDF) of the standard normal distribution N(0,1).
		/// The CDF gives the probability that a standard normal variable Z is less than or equal to a given value.
		/// </summary>
		/// <param name="z">Z-score, the number of standard deviations from the mean.</param>
		/// <returns>Probability P(Z ≤ z).</returns>
		private static double NormalCDF(double z)
		{
			// Coefficients for the Abramowitz & Stegun approximation
			const double a1 = 0.254829592, a2 = -0.284496736, a3 = 1.421413741;
			const double a4 = -1.453152027, a5 = 1.061405429, p = 0.3275911;

			double sign = Math.Sign(z);
			double x = Math.Abs(z) / Math.Sqrt(2.0);

			double t = 1.0 / (1.0 + p * x);
			double y = 1.0 - (((((a5 * t + a4) * t + a3) * t + a2) * t + a1) * t) * Math.Exp(-x * x);

			return 0.5 * (1.0 + sign * y);
		}

		/// <summary>
		/// Calculates the minimum number of independent attempts required to achieve at least one success
		/// given a per-attempt success probability <paramref name="hitRate"/>. 
		/// 
		/// The calculation ensures that the probability of failing all attempts does not exceed 
		/// <paramref name="confidence"/>. Formally, it finds the smallest integer N such that:
		/// <code>
		/// 1 - (1 - hitRate)^N ≥ confidence
		/// </code>
		/// 
		/// Special cases are handled explicitly:
		/// <list type="bullet">
		/// <item>If <paramref name="hitRate"/> is effectively 0, success is practically impossible 
		///      and the method returns <see cref="int.MaxValue"/>.</item>
		/// <item>If <paramref name="hitRate"/> is effectively 1, success is guaranteed on the first attempt, 
		///      so the method returns 1.</item>
		/// </list>
		/// </summary>
		/// <param name="hitRate">
		/// Probability of success for a single attempt. Must be in the range [0.0, 1.0].
		/// </param>
		/// <param name="confidence">
		/// Desired minimum probability of achieving at least one success across all attempts. 
		/// Must be strictly between 0 and 1. Default is 0.9999 (i.e., 99.99% confidence).
		/// </param>
		/// <returns>
		/// The minimum number of attempts required to achieve at least one success with the specified confidence.
		/// Returns:
		/// - <see cref="int.MaxValue"/> if success is effectively impossible (hitRate ≈ 0),
		/// - 1 if success is essentially guaranteed (hitRate ≈ 1),
		/// - or the smallest integer ≥ the calculated number of attempts for general cases.
		/// </returns>
		public static int GetHitAttempts(double hitRate, double confidence = 0.9999)
		{
			if (hitRate < 0.0 || hitRate > 1.0)
			{
				throw new ArgumentOutOfRangeException(nameof(hitRate),
					"Hit rate must be between 0.0 and 1.0");
			}

			if (confidence <= 0.0 || confidence >= 1.0)
			{
				throw new ArgumentOutOfRangeException(nameof(confidence),
					"Confidence must be in the exclusive range (0, 1)");
			}

			if (hitRate <= double.Epsilon)
				return int.MaxValue;

			if ((1.0 - hitRate) <= double.Epsilon)
				return 1;

			double attempts = Math.Ceiling(Math.Log(1.0 - confidence) / Math.Log(1.0 - hitRate));

			if (attempts >= int.MaxValue)
				return int.MaxValue;

			return Math.Max((int)attempts, 1);
		}

		/// <summary>
		/// Generates a random number from a normal distribution with the specified mean and standard deviation.
		/// Uses the Box-Muller transform to convert uniformly distributed random values into normally distributed ones.
		/// Supports caching of the second generated value for efficiency.
		/// </summary>
		/// <param name="mean">Mean (μ) of the target normal distribution.</param>
		/// <param name="stdDev">Standard deviation (σ) of the distribution. Must be non-negative.</param>
		/// <param name="cache">
		/// Reference to a nullable double used to store one of the two generated standard normal values.
		/// If a cached value exists, it is used directly to save computation.
		/// </param>
		/// <returns>A random value sampled from N(mean, σ²).</returns>
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
			// using Box-Muller transform
			double u1 = 1.0 - CoreTools.NextDouble(); // avoid Math.Log(0)
			double u2 = CoreTools.NextDouble();

			double radius = Math.Sqrt(-2.0 * Math.Log(u1));
			double angle = 2.0 * Math.PI * u2;

			double z1 = radius * Math.Cos(angle);
			double z2 = radius * Math.Sin(angle);

			// cache for the next call
			cache = z2;

			return mean + (z1 * stdDev);
		}
	}
}

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
		/// Calculates the probability that a normally distributed random variable,
		/// defined by the specified <paramref name="mean"/> and <paramref name="stdDev"/>,
		/// will fall inside the interval [Min, Max].
		/// </summary>
		/// <param name="mean">
		/// Mean (μ) of the normal distribution.
		/// </param>
		/// <param name="stdDev">
		/// Standard deviation (σ) of the distribution. Must be positive.
		/// A value close to zero is treated as a degenerate distribution.
		/// </param>
		/// <param name="range">
		/// A tuple specifying the lower (Min) and upper (Max) bounds of the interval.
		/// </param>
		/// <returns>
		/// A value in the range [0, 1] representing the probability that a sample from
		/// N(mean, σ²) lies within [Min, Max].
		///
		/// <para>
		/// • Returns <c>0</c> if the interval is invalid (Min ≥ Max).  
		/// • If <paramref name="stdDev"/> is effectively zero:
		///   <br/>— Returns <c>1</c> if <paramref name="mean"/> lies inside the interval.
		///   <br/>— Returns <c>0</c> otherwise.
		/// </para>
		/// </returns>
		public static double GetRangeHitProbability(double mean, double stdDev, (double Min, double Max) range)
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
		/// Computes the minimum number of independent trials required to achieve
		/// at least one success with a specified overall confidence level,
		/// given the per-trial success probability <paramref name="pHit"/>.
		/// 
		/// The calculation finds the smallest integer N such that:
		/// <code>
		/// 1 - (1 - pHit)^N ≥ confidence
		/// </code>
		/// or equivalently:
		/// <code>
		/// (1 - pHit)^N ≤ 1 - confidence
		/// </code>
		/// </summary>
		/// <param name="pHit">
		/// Probability of success for a single independent trial (range: 0–1).
		/// </param>
		/// <param name="confidence">
		/// Required probability of achieving at least one success across all trials.
		/// Must be in the exclusive range (0, 1). Default is 0.9999 (99.99%).
		/// </param>
		/// <returns>
		/// The minimum number of trials necessary to meet the desired confidence.
		/// </returns>
        public static int GetRequiredAttempts(double pHit, double confidence = 0.9999)
		{
			if (pHit < 0.0 || pHit > 1.0)
			{
				throw new ArgumentOutOfRangeException(nameof(pHit),
					"Per-attempt success probability must be between 0.0 and 1.0");
			}

			if (confidence <= 0.0 || confidence >= 1.0)
			{
				throw new ArgumentOutOfRangeException(nameof(confidence),
					"Confidence must be in the exclusive range (0, 1).");
			}

			// No realistic chance of success
			if (pHit <= double.Epsilon)
				return int.MaxValue;

			// Guaranteed success on first attempt
			if ((1.0 - pHit) <= double.Epsilon)
				return 1;

			// Compute required attempts
			long attempts = (long)Math.Ceiling(Math.Log(1.0 - confidence) / Math.Log(1.0 - pHit));

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

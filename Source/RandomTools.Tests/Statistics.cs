namespace RandomTools.Tests
{
	/// <summary>
	/// Provides helper statistical methods for analyzing random delay samples.
	/// Includes computation of mean, variance, standard deviation, standard error of the mean (SEM),
	/// and confidence interval deltas for various confidence levels.
	/// </summary>
	internal static class Statistics
	{
		/// <summary>
		/// Z-values corresponding to confidence levels, indexed by <see cref="ConfidenceLevel"/>.
		/// These values are used to compute confidence interval deltas (half-widths).
		/// </summary>
		private static readonly double[] ZValues =
		{
			1.281551565544600, // Confidence80
            1.439531470938460, // Confidence85
            1.644853626951470, // Confidence90
            1.959963984540050, // Confidence95
            2.170090377584560, // Confidence97
            2.326347874040840, // Confidence98
            2.575829303548900, // Confidence99
            2.807033768343810, // Confidence995
            3.290526731491930  // Confidence999
        };

		/// <summary>
		/// Ensures that the ZValues array length matches the number of <see cref="ConfidenceLevel"/> values.
		/// Throws an exception if they do not match.
		/// </summary>
		static Statistics()
		{
			if (ZValues.Length != Enum.GetValues<ConfidenceLevel>().Length)
			{
				throw new InvalidOperationException(
					$"ZValues array length ({ZValues.Length}) does not match number of ConfidenceLevel values ({Enum.GetValues<ConfidenceLevel>().Length}).");
			}
		}

		/// <summary>
		/// Computes the confidence interval delta (half-width) for a given confidence level and standard error.
		/// </summary>
		/// <param name="level">The desired confidence level.</param>
		/// <param name="sem">The standard error of the mean.</param>
		/// <returns>The confidence interval delta.</returns>
		public static double ConfidenceDelta(ConfidenceLevel level, double sem)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(sem);

			double zValue = ZValues[(int)level];
			return zValue * sem;
		}

		/// <summary>
		/// Analyzes a collection of samples and returns the mean, variance, standard deviation, and sample count.
		/// Uses Welford's online algorithm for numerically stable computation.
		/// </summary>
		/// <param name="samples">A sequence of double values representing the data samples.</param>
		/// <returns>
		/// A tuple containing:
		/// - Mean of the samples
		/// - Variance of the samples
		/// - Standard deviation of the samples
		/// - Number of samples analyzed
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="samples"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if fewer than 2 samples are provided.</exception>
		public static (double Mean, double Variance, double StandardDeviation, int Count) AnalyzeSamples(IEnumerable<double> samples)
		{
			ArgumentNullException.ThrowIfNull(samples);

			int count = 0;
			double mean = 0.0;
			double m2 = 0.0;

			foreach (double next in samples)
			{
				count++;

				double delta = next - mean;
				mean += delta / count;
				m2 += delta * (next - mean);
			}

			if (count < 2)
			{
				throw new ArgumentException(
					"At least two samples are required to compute variance and standard deviation.",
					nameof(samples));
			}

			double variance = m2 / (count - 1);
			return (mean, variance, Math.Sqrt(variance), count);
		}

		/// <summary>
		/// Computes the Standard Error of the Mean (SEM), which indicates
		/// how much the sample mean is expected to vary from the true population mean.
		/// </summary>
		/// <param name="stdDev">The standard deviation of the sample or population.</param>
		/// <param name="sampleCount">The number of independent samples.</param>
		/// <returns>The standard error of the mean.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="stdDev"/> is negative or <paramref name="sampleCount"/> is less than 1.
		/// </exception>
		public static double StandardErrorOfMean(double stdDev, int sampleCount)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(stdDev);
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(sampleCount, 0);

			return stdDev / Math.Sqrt(sampleCount);
		}

		/// <summary>
		/// Estimates the order <c>n</c> of a Bates distribution from the sample variance.
		/// </summary>
		/// <param name="variance">Variance of the Bates-distributed sample.</param>
		/// <param name="bounds">Min and Max of the underlying Bates distribution.</param>
		/// <returns>Estimated number of uniform variables averaged (n).</returns>
		public static double EstimateBatesOrder(double variance, (double Min, double Max) bounds)
		{
			double range = bounds.Max - bounds.Min;
			double n = range * range / (12.0 * variance);

			return n;
		}
	}

	/// <summary>
	/// Represents the supported confidence levels for computing confidence intervals.
	/// </summary>
	internal enum ConfidenceLevel
	{
		Confidence80,
		Confidence85,
		Confidence90,
		Confidence95,
		Confidence97,
		Confidence98,
		Confidence99,
		Confidence995,
		Confidence999
	}
}
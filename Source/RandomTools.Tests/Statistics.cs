namespace RandomTools.Tests
{
	/// <summary>
	/// Provides helper statistical methods for analyzing random delay samples.
	/// </summary>
	internal static class Statistics
	{
		// Z-values for confidence intervals, indexed by ConfidenceLevel enum.
		private static readonly double[] ZValues =
		[
			1.281551565544600, // Confidence80
            1.439531470938460, // Confidence85
            1.644853626951470, // Confidence90
            1.959963984540050, // Confidence95
            2.170090377584560, // Confidence97
            2.326347874040840, // Confidence98
            2.575829303548900, // Confidence99
            2.807033768343810, // Confidence995
            3.290526731491930  // Confidence999
        ];

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
		public static double GetConfidenceDelta(ConfidenceLevel level, double sem)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(sem);

			double z = ZValues[(int)level];
			return z * sem;
		}

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
		/// Computes the arithmetic mean of a sequence of <see cref="TimeSpan"/> values.
		/// Uses a numerically stable single-pass algorithm (incremental mean).
		/// </summary>
		/// <param name="samples">A sequence of sampled <see cref="TimeSpan"/> values.</param>
		/// <returns>The mean of the samples as a <see cref="TimeSpan"/>.</returns>
		public static TimeSpan Mean(IEnumerable<TimeSpan> samples)
		{
			double meanTicks = 0.0;
			long count = 0;

			foreach (TimeSpan next in samples)
			{
				double ticks = (next.Ticks - meanTicks) / ++count;
				meanTicks += ticks;
			}

			return TimeSpan.FromTicks((long)Math.Round(meanTicks));
		}

		/// <summary>
		/// Computes the Standard Error of the Mean (SEM), which describes
		/// how much the sample mean is expected to vary from the true mean.
		/// </summary>
		/// <param name="stdDev">The theoretical or observed standard deviation.</param>
		/// <param name="sampleCount">The number of independent samples collected.</param>
		/// <returns>The standard error of the mean.</returns>
		public static double StandardErrorOfMean(double stdDev, int sampleCount)
		{
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(sampleCount, 0);
			return stdDev / Math.Sqrt(sampleCount);
		}
	}

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

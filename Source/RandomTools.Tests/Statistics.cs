namespace RandomTools.Tests
{
	/// <summary>
	/// Provides helper statistical methods for analyzing random delay samples.
	/// </summary>
	internal static class Statistics
	{
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
		/// <param name="trials">The number of independent samples collected.</param>
		/// <returns>The standard error of the mean.</returns>
		public static double SEM(double stdDev, int trials)
		{
			return stdDev / Math.Sqrt(trials);
		}

		/// <summary>
		/// Provides theoretical statistics for known distributions.
		/// </summary>
		public static class Theory
		{
			/// <summary>
			/// Theoretical means for known distributions.
			/// </summary>
			public static class Mean
			{
				/// <summary>
				/// Computes the theoretical mean of a Bates(a, b, N) distribution.
				/// The mean is always the midpoint of the interval.
				/// </summary>
				public static double Bates(double min, double max)
				{
					return (min + max) / 2.0;
				}
			}

			/// <summary>
			/// Theoretical standard deviation formulas for distributions.
			/// </summary>
			public static class StandardDeviation
			{
				/// <summary>
				/// Computes the theoretical standard deviation of a Bates distribution.
				/// Formula:
				///     σ = (b - a) / sqrt(12 * N)
				/// where:
				///   - a = minimum
				///   - b = maximum
				///   - N = number of uniform samples averaged
				/// </summary>
				public static double Bates(double min, double max, int samples)
				{
					return (max - min) / Math.Sqrt(12.0 * samples);
				}
			}
		}
	}
}

namespace RandomTools.Tests
{
	/// <summary>
	/// Provides statistical helper methods for analyzing numeric sample collections.
	/// Includes:
	/// - Mean, variance, and standard deviation calculation using Welford's numerically stable algorithm.
	/// - Standard error of the mean (SEM) calculation.
	/// - Confidence interval half-width (delta) calculation.
	/// - Histogram generation with max-bin tracking.
	/// - Estimation of Bates distribution order from sample variance.
	/// </summary>
	internal static class Statistics
	{
		/// <summary>
		/// Z-values corresponding to each <see cref="ConfidenceLevel"/> used for computing
		/// confidence intervals around the sample mean. These values come from the standard
		/// normal distribution and correspond to the one-sided z-score for the given confidence.
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
		/// Static constructor verifies that the Z-values array length matches the number
		/// of <see cref="ConfidenceLevel"/> enum entries. Throws <see cref="InvalidOperationException"/>
		/// if lengths do not match.
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
		/// Computes the confidence interval half-width (delta) for a given confidence level and standard error of the mean.
		/// The resulting delta can be used to assert that a sample mean is within ±delta of the expected mean.
		/// </summary>
		/// <param name="level">Confidence level (e.g., 95%).</param>
		/// <param name="standardErrorOfMean">Standard error of the mean (SD / √n).</param>
		/// <returns>Half-width of the confidence interval.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="standardErrorOfMean"/> is negative.</exception>
		public static double ConfidenceDelta(ConfidenceLevel level, double standardErrorOfMean)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(standardErrorOfMean);
			return ZValues[(int)level] * standardErrorOfMean;
		}

		/// <summary>
		/// Computes descriptive statistics for a sequence of numeric samples using Welford’s one-pass algorithm,
		/// which is numerically stable and efficient for large datasets.
		/// </summary>
		/// <param name="samples">The numeric samples to analyze.</param>
		/// <returns>
		/// A tuple containing:
		/// <list type="bullet">
		///   <item><description><c>Mean</c> — Arithmetic mean (Σx / n).</description></item>
		///   <item><description><c>Variance</c> — Sample variance (Σ(x-mean)² / (n-1)).</description></item>
		///   <item><description><c>StandardDeviation</c> — Square root of the sample variance.</description></item>
		///   <item><description><c>StandardError</c> — Standard error of the mean (SD / √n).</description></item>
		///   <item><description><c>Skewness</c> — Measure of asymmetry of the distribution. 0 for symmetric distributions.</description></item>
		///   <item><description><c>Count</c> — Number of samples analyzed.</description></item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Skewness is calculated as:  
		/// <c>Skewness = (1/n) * Σ[(x_i - mean)/SD]^3</c>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="samples"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown when fewer than two samples are provided.</exception>
		public static (double Mean, double Variance, double StandardDeviation, double StandardError, double Skewness, int Count) AnalyzeSamples(IEnumerable<double> samples)
		{
			ArgumentNullException.ThrowIfNull(samples);

			double[] values = [.. samples];

			if (values.Length < 2)
			{
				throw new ArgumentException(
					"At least two samples are required to compute variance.",
					nameof(samples));
			}

			int count = 0;
			double mean = 0.0;
			double m2 = 0.0;

			foreach (double next in values)
			{
				count++;

				double delta = next - mean;
				mean += delta / count;
				m2 += delta * (next - mean);
			}

			double variance = m2 / (count - 1);
			double standardDeviation = Math.Sqrt(variance);
			double standardError = standardDeviation / Math.Sqrt(count);
			double skewness = values.Sum(x => Math.Pow((x - mean) / standardDeviation, 3.0)) / count;

			return (mean, variance, standardDeviation, standardError, skewness, count);
		}

		/// <summary>
		/// Estimates the order <c>n</c> of a Bates distribution from the sample variance.
		/// The Bates distribution is the mean of <c>n</c> independent uniform random variables.
		/// </summary>
		/// <param name="variance">Sample variance of Bates-distributed data.</param>
		/// <param name="bounds">Minimum and maximum of the underlying uniform distribution.</param>
		/// <returns>Estimated number of uniform variables averaged (n).</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="variance"/> is negative.</exception>
		public static double EstimateBatesOrder(double variance, (double Min, double Max) bounds)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(variance);
			
			double range = bounds.Max - bounds.Min;
			return range * range / (12.0 * variance);
		}

		/// <summary>
		/// Estimates the mode of a sample set by identifying the histogram bin with the maximum count.
		/// Returns the midpoint of that bin.
		/// </summary>
		/// <param name="samples">Sample values.</param>
		/// <param name="bins">Number of histogram bins.</param>
		/// <returns>Midpoint of the most frequent bin.</returns>
		public static double EstimateMode(IEnumerable<double> samples, int bins)
		{
			ArgumentNullException.ThrowIfNull(samples);
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bins);

			var hist = ComputeHistogram(samples, bins);
			return Math.FusedMultiplyAdd(hist.MaxBin + 0.5, hist.Width, hist.Min);
		}

		/// <summary>
		/// Computes a histogram for numeric samples.
		/// </summary>
		/// <param name="samples">Numeric samples.</param>
		/// <param name="bins">Number of bins (default 100).</param>
		/// <returns>A <see cref="Histogram"/> object containing counts per bin and max-bin info.</returns>
		public static Histogram ComputeHistogram(IEnumerable<double> samples, int bins = 100)
		{
			ArgumentNullException.ThrowIfNull(samples);
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bins);

			double min = samples.Min();
			double max = samples.Max();

			var hist = new Histogram
			{
				Bins = bins,
				Min = min,
				Max = max,
				Width = (max - min) / bins,
				Counts = new int[bins]
			};

			foreach (double next in samples)
			{
				int idx = (int)((next - hist.Min) / hist.Width);
				if (idx >= hist.Bins)
					idx = hist.Bins - 1;

				hist.Counts[idx]++;
				if (hist.Counts[idx] > hist.MaxHits)
				{
					hist.MaxHits = hist.Counts[idx];
					hist.MaxBin = idx;
				}
			}

			return hist;
		}
	}

	/// <summary>
	/// Represents a histogram of numeric values with fixed bins and counts per bin.
	/// Tracks the bin with the maximum count.
	/// </summary>
	internal sealed class Histogram
	{
		public int Bins { get; init; }
		public double Min { get; init; }
		public double Max { get; init; }
		public double Width { get; init; }
		public int[] Counts { get; init; } = [];
		public int MaxHits { get; set; }
		public int MaxBin { get; set; } = -1;
	}

	/// <summary>
	/// Supported confidence levels for computing confidence intervals.
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

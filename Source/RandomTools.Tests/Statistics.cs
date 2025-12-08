namespace RandomTools.Tests
{
	/// <summary>
	/// Provides statistical helper methods for analyzing numeric sample collections.
	/// Includes:
	/// - Mean, variance, and standard deviation calculation using Welford's algorithm.
	/// - Standard error of the mean (SEM) calculation.
	/// - Confidence interval delta (half-width) calculation.
	/// - Histogram generation with max-bin tracking.
	/// - Estimation of Bates distribution order from sample variance.
	/// </summary>
	internal static class Statistics
	{
		/// <summary>
		/// Z-values corresponding to each <see cref="ConfidenceLevel"/>.
		/// These are used to compute the confidence interval half-width (delta) for a given SEM.
		/// The array is indexed by the enum value of <see cref="ConfidenceLevel"/>.
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
		/// Ensures that the Z-values array matches the number of <see cref="ConfidenceLevel"/> entries.
		/// Throws an exception if the lengths do not match.
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
		/// Computes the confidence interval half-width (delta) for a given confidence level and SEM.
		/// </summary>
		/// <param name="level">Confidence level (e.g., 95%).</param>
		/// <param name="standardErrorOfMean">Standard error of the mean (SEM).</param>
		/// <returns>Half-width of the confidence interval.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="standardErrorOfMean"/> is negative.</exception>
		public static double ConfidenceDelta(ConfidenceLevel level, double standardErrorOfMean)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(standardErrorOfMean);

			return ZValues[(int)level] * standardErrorOfMean;
		}

		/// <summary>
		/// Computes the mean, variance, standard deviation, and sample count for a collection of numeric samples.
		/// Uses Welford's online algorithm for numerically stable variance calculation.
		/// </summary>
		/// <param name="samples">Collection of numeric samples.</param>
		/// <returns>
		/// A tuple containing:
		/// - Mean of the samples.
		/// - Sample variance (dividing by n-1).
		/// - Standard deviation (sqrt of variance).
		/// - Number of samples analyzed.
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

			double variance = m2 / (count - 1); // Sample variance
			return (mean, variance, Math.Sqrt(variance), count);
		}

		/// <summary>
		/// Computes the standard error of the mean (SEM) given sample standard deviation and count.
		/// SEM quantifies the expected deviation of the sample mean from the true population mean.
		/// </summary>
		/// <param name="stdDev">Sample standard deviation.</param>
		/// <param name="sampleCount">Number of independent samples.</param>
		/// <returns>Standard error of the mean.</returns>
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
		/// The Bates distribution is the mean of <c>n</c> independent uniform random variables.
		/// </summary>
		/// <param name="variance">Sample variance of the Bates-distributed data.</param>
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
		/// Computes a histogram for numeric samples within specified bounds and bin count.
		/// Tracks the number of samples in each bin and the bin with the maximum count.
		/// </summary>
		/// <param name="samples">Collection of numeric samples.</param>
		/// <param name="bounds">Minimum and maximum bounds of the histogram.</param>
		/// <param name="bins">Number of bins (default 100).</param>
		/// <returns>
		/// A <see cref="Histogram"/> object containing:
		/// - Counts per bin.
		/// - Bin width.
		/// - Index and value range of the bin with the maximum count.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="samples"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="bins"/> is less than 1 or if <paramref name="bounds"/> are invalid.
		/// </exception>
		public static Histogram ComputeHistogram(IEnumerable<double> samples, (double Min, double Max) bounds, int bins = 100)
		{
			ArgumentNullException.ThrowIfNull(samples);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(bounds.Min, bounds.Max);
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bins);

			var hist = new Histogram
			{
				Bins = bins,
				Bounds = bounds,
				Width = (bounds.Max - bounds.Min) / bins,
				Counts = new int[bins]
			};

			foreach (double next in samples)
			{
				if (next < bounds.Min || next > bounds.Max)
					continue;

				int idx = (int)((next - bounds.Min) / hist.Width);
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
		/// <summary>Number of bins in the histogram.</summary>
		public int Bins { get; init; }

		/// <summary>Width of each bin.</summary>
		public double Width { get; init; }

		/// <summary>Minimum and maximum bounds of the histogram.</summary>
		public (double Min, double Max) Bounds { get; init; }

		/// <summary>Count of samples in each bin.</summary>
		public int[] Counts { get; init; } = Array.Empty<int>();

		/// <summary>Maximum count in any single bin.</summary>
		public int MaxHits { get; set; }

		/// <summary>Index of the bin with the maximum count.</summary>
		public int MaxBin { get; set; } = -1;

		/// <summary>
		/// Returns the value range covered by a specific bin.
		/// </summary>
		/// <param name="binIndex">Index of the bin (0-based).</param>
		/// <returns>Tuple (Min, Max) representing the value range of the bin.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="binIndex"/> is outside valid range.</exception>
		public (double Min, double Max) GetBinRange(int binIndex)
		{
			if (binIndex < 0 || binIndex >= Bins)
			{
				throw new ArgumentOutOfRangeException(
					nameof(binIndex),
					"Bin-index is out of bounds.");
			}

			double binMin = Math.FusedMultiplyAdd(binIndex, Width, Bounds.Min);
			double binMax = (binIndex + 1 == Bins) 
				? Bounds.Max 
				: Math.FusedMultiplyAdd(binIndex + 1, Width, Bounds.Min);

			return (binMin, binMax);
		}

		/// <summary>
		/// Returns the value range of the bin with the maximum count.
		/// </summary>
		public (double Min, double Max)? GetMaxBinRange()
		{
			return MaxBin >= 0
				? GetBinRange(MaxBin)
			    : null;
		}
	}

	/// <summary>Supported confidence levels for computing confidence intervals.</summary>
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
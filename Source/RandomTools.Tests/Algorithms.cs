namespace RandomTools.Tests
{
	/// <summary>
	/// Provides statistical utility algorithms for test verification.
	/// </summary>
	internal static class Algorithms
	{
		/// <summary>
		/// Critical Chi-Square values for α = 0.01 and degrees of freedom 1–100.
		/// Index = df - 1.
		/// </summary>
		private static readonly double[] ChiSquareCritical =
		{
			6.635, 9.210, 11.345, 13.277, 15.086, 16.812, 18.475, 20.090, 21.666, 23.209,
			24.725, 26.217, 27.688, 29.141, 30.578, 32.000, 33.409, 34.805, 36.191, 37.566,
			38.932, 40.289, 41.638, 42.980, 44.314, 45.642, 46.963, 48.278, 49.588, 50.892,
			52.191, 53.484, 54.773, 56.057, 57.337, 58.612, 59.884, 61.151, 62.415, 63.675,
			64.931, 66.185, 67.435, 68.682, 69.927, 71.169, 72.409, 73.646, 74.881, 76.113,
			77.343, 78.571, 79.797, 81.021, 82.243, 83.463, 84.681, 85.898, 87.113, 88.326,
			89.538, 90.748, 91.957, 93.165, 94.371, 95.576, 96.780, 97.983, 99.185, 100.386,
			101.586, 102.785, 103.983, 105.181, 106.378, 107.574, 108.769, 109.964, 111.158, 112.352,
			113.545, 114.737, 115.929, 117.120, 118.311, 119.501, 120.691, 121.880, 123.069, 124.257,
			125.445, 126.632, 127.819, 129.006, 130.192, 131.378, 132.563, 133.748, 134.933, 136.118
		};

		/// <summary>
		/// Returns the critical chi-square value for α = 0.01 and the given degrees of freedom.
		/// </summary>
		/// <param name="df">Degrees of freedom (1–100).</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when df is outside 1–100.</exception>
		public static double CriticalChiSquare(int df)
		{
			if (df < 1 || df > ChiSquareCritical.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(df), "Degrees of freedom must be in range 1–100.");
			}

			return ChiSquareCritical[df - 1];
		}

		/// <summary>
		/// Computes the Chi-Square statistic for a set of observed frequencies
		/// against a uniform expected distribution.
		/// </summary>
		/// <param name="trials">Total number of generated samples.</param>
		/// <param name="observed">Observed counts per bin.</param>
		/// <returns>The chi-square value.</returns>
		/// <exception cref="ArgumentNullException">Thrown when observed is null.</exception>
		/// <exception cref="ArgumentException">Thrown when observed is empty.</exception>
		public static double ChiSquare(int trials, IEnumerable<int> observed)
		{
			ArgumentNullException.ThrowIfNull(observed);

			var theList = observed as IList<int> ?? [.. observed];

			if (theList.Count == 0)
			{
				throw new ArgumentException(
					"Observed collection cannot be empty.", 
					nameof(observed));
			}

			double expected = (double)trials / theList.Count;
			double chiSquare = 0d;

			for (int i = 0; i < theList.Count; i++)
			{
				double diff = theList[i] - expected;
				chiSquare += diff * diff / expected;
			}

			return chiSquare;
		}
	}
}

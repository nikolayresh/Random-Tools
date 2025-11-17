namespace RandomTools.Tests
{
	internal static class ValuesProvider
	{
		/// <summary>
		/// Provides a collection of non-finite double constants: NaN, PositiveInfinity, and NegativeInfinity.
		/// </summary>
		public static IEnumerable<double> NonFinite()
		{
			yield return double.NaN;
			yield return double.PositiveInfinity;
			yield return double.NegativeInfinity;
		}

		/// <summary>
		/// Provides the double value 0.0 and the smallest positive normal value (Epsilon).
		/// </summary>
		public static IEnumerable<double> ZeroLike()
		{
			yield return 0d;
			yield return double.Epsilon;
		}
	}
}

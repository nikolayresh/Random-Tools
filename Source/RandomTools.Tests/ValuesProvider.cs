using RandomTools.Core;

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

		/// <summary>
		/// Retrieves an enumerable collection of all defined values in the <see cref="TimeUnit"/> enumeration.
		/// </summary>
		/// <remarks>This method is useful for iterating over all possible time units defined in the <see
		/// cref="TimeUnit"/> enumeration.</remarks>
		/// <returns>An <see cref="IEnumerable{T}"/> containing all values of the <see cref="TimeUnit"/> enumeration.</returns>
		public static IEnumerable<TimeUnit> TimeUnits()
		{
			foreach (var unit in Enum.GetValues<TimeUnit>())
			{
				yield return unit;
			}
		}
	}
}

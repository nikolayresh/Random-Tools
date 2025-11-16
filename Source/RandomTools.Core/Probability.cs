namespace RandomTools.Core
{
	/// <summary>
	/// Provides commonly used probability constants.
	/// </summary>
	public static class Probability
	{
		/// <summary>
		/// Represents a probability of 1.0 (100%).
		/// Use when an event is guaranteed to occur.
		/// </summary>
		public const double Certain = 1.0;

		/// <summary>
		/// Represents a probability of 0.0 (0%).
		/// Use when an event is guaranteed not to occur.
		/// </summary>
		public const double Impossible = 0.0;
	}
}
namespace RandomTools.Tests
{
	/// <summary>
	/// Provides centralized exception message constants and formatting helpers.
	/// </summary>
	/// <remarks>
	/// Using predefined messages and helpers ensures consistent and maintainable exception reporting
	/// across the application. This class includes constants for common error scenarios and methods
	/// to format values in exception messages.
	/// </remarks>
	internal static class ExceptionMessages
	{
		/// <summary>
		/// Message used when a delay range is invalid or too narrow to be meaningful.
		/// </summary>
		public const string RangeIsTooShort = "Invalid delay range - the interval is too narrow.";

		/// <summary>
		/// Formats an object as a string, optionally enclosing it in parentheses.
		/// </summary>
		/// <param name="value">The value to format. Can be <c>null</c>.</param>
		/// <param name="enclosed">If <c>true</c>, the output is wrapped in parentheses.</param>
		/// <returns>The formatted string, or <c>null</c> if <paramref name="value"/> is <c>null</c>.</returns>
		public static string? Value(object value, bool enclosed) =>
			enclosed ? $"({value})" : value.ToString();
	}
}

namespace RandomTools.Core
{
	/// <summary>
	/// Represents an error that occurs when an options object implementing <see cref="IOptionsBase"/>
	/// is in an invalid state or fails validation.
	/// </summary>
	/// <remarks>
	/// This exception can be thrown by any class implementing <see cref="IOptionsBase"/> 
	/// to indicate that its configuration is invalid, inconsistent, or violates constraints.
	/// Examples include missing required values, invalid combinations of settings, or 
	/// attempts to generate output that is impossible given the current configuration.
	/// </remarks>
	public class OptionsValidationException : Exception
	{
		public OptionsValidationException(string? message)
			: base(message) { }

		public OptionsValidationException(string? message, Exception? innerException)
			: base(message, innerException) { }
	}
}

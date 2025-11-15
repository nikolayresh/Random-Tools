using RandomTools.Core.Options;

namespace RandomTools.Core.Exceptions
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
	public class OptionsValidationException : RandomToolException
	{
		public OptionsValidationException(IOptionsBase options, string? message)
			: base(options, message) { }
	}
}

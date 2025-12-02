using RandomTools.Core.Options;

namespace RandomTools.Core.Exceptions
{
	/// <summary>
	/// Exception thrown when an <see cref="IOptionsBase"/> instance contains invalid
	/// or inconsistent configuration values.
	/// </summary>
	public sealed class OptionsValidationException : RandomToolException
	{
#pragma warning disable IDE0290
		/// <summary>
		/// Initializes a new instance of the <see cref="OptionsValidationException"/> class
		/// using the specified invalid options and a descriptive error message.
		/// </summary>
		/// <param name="options">The options object that failed validation.</param>
		/// <param name="message">A message describing the validation error.</param>
		public OptionsValidationException(IOptionsBase options, string? message)
			: base(options, message) { }
#pragma warning restore IDE0290
	}
}
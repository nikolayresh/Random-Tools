namespace RandomTools.Core.Options
{
	/// <summary>
	/// Represents a base interface for configuration options in the library.
	/// Implementing types must provide a <see cref="Validate"/> method
	/// to ensure that their values are logically consistent and meet any
	/// required constraints
	/// </summary>
	public interface IOptionsBase
	{
		/// <summary>
		/// Validates the option values to ensure they are correct and consistent.
		/// Implementing classes should throw an appropriate exception if validation fails.
		/// </summary>
		/// <exception cref="OptionsValidationException">
		/// Thrown when the option values are invalid or inconsistent.
		/// </exception>
		void Validate();

		/// <summary>
		/// Creates a new instance of the options object with the same settings as the current instance.
		/// </summary>
		/// <returns>A new <see cref="IOptionsBase"/> instance that is a copy of the current object.</returns>
		IOptionsBase Clone();
	}
}

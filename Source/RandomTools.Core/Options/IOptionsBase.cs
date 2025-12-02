namespace RandomTools.Core.Options
{
	/// <summary>
	/// Represents the base interface for configuration options in the library.
	/// Implementing types must provide mechanisms for validation and cloning to
	/// ensure safe, consistent usage of options throughout the library.
	/// </summary>
	public interface IOptionsBase
	{
		/// <summary>
		/// Validates the current option values to ensure they are correct, consistent, 
		/// and meet any required constraints.
		/// <para>
		/// This method should be called before using the options in operations
		/// such as random value generation or delay computations to prevent
		/// runtime errors or undefined behavior.
		/// </para>
		/// </summary>
		/// <exception cref="OptionsValidationException">
		/// Thrown when one or more option values are invalid or inconsistent,
		/// for example if a minimum value is greater than a maximum, or required
		/// parameters are missing.
		/// </exception>
		void Validate();

		/// <summary>
		/// Creates a new copy of the current options instance with the same configuration.
		/// <para>
		/// Cloning is useful when you need a separate instance to modify without
		/// affecting the original, or when using options as keys in collections
		/// that rely on immutability and value equality.
		/// </para>
		/// </summary>
		/// <returns>
		/// A new <see cref="IOptionsBase"/> instance that is a deep copy of the current options.
		/// </returns>
		IOptionsBase Clone();
	}
}

namespace RandomTools.Core.Options
{
	/// <summary>
	/// A "no-op" options class that implements <see cref="IOptionsBase"/>.
	/// Can be used as a default or placeholder when no options are required.
	/// </summary>
	public sealed class NullOptions : IOptionsBase
	{
		/// <summary>
		/// Validates the options.
		/// This implementation does nothing.
		/// </summary>
		public void Validate()
		{
			// No validation required
		}

		public IOptionsBase Clone() => new NullOptions();

		/// <summary>
		/// Returns a constant hash code.
		/// </summary>
		public override int GetHashCode() => 0;

		/// <summary>
		/// Always equal to another <see cref="NullOptions"/> instance.
		/// </summary>
		public override bool Equals(object? obj) => obj is NullOptions;
	}
}
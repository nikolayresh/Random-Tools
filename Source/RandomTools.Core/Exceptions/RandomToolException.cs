using RandomTools.Core.Options;

namespace RandomTools.Core.Exceptions
{
	/// <summary>
	/// Base type for all exceptions thrown by RandomTools components.
	/// </summary>
	/// <remarks>
	/// This exception stores the <see cref="IOptionsBase"/> instance that caused the failure.
	/// A cloned copy of the options is preserved to capture the exact configuration used when
	/// the error occurred. This makes diagnosing invalid configurations, invalid ranges,
	/// or misused parameters easier, especially during debugging or logging.
	/// </remarks>
	public abstract class RandomToolException : Exception
	{
#pragma warning disable IDE0290
		/// <summary>
		/// Creates a new <see cref="RandomToolException"/> associated with a specific options object.
		/// </summary>
		/// <param name="options">
		/// The options used for the operation that failed.  
		/// This value cannot be <c>null</c>.
		/// </param>
		/// <param name="message">
		/// A human-readable description of the problem.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="options"/> is <c>null</c>.
		/// </exception>
		protected RandomToolException(IOptionsBase options, string? message) : base(message)
#pragma warning restore IDE0290
		{
			ArgumentNullException.ThrowIfNull(options);

			// Preserve the configuration exactly as it was at the moment of failure.
			Options = options.Clone();
		}

		/// <summary>
		/// Gets a copy of the options that were active when the exception was created.
		/// </summary>
		/// <remarks>
		/// This makes it easier to understand the context of the failure without relying on
		/// external state. The stored options cannot be modified externally.
		/// </remarks>
		public IOptionsBase Options { get; }
	}
}
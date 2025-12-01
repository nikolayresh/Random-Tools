using RandomTools.Core.Options;

namespace RandomTools.Core.Random
{
	/// <summary>
	/// Base class for all random value generators with configurable options.
	/// Provides common methods for generating random values, including exclusions.
	/// </summary>
	/// <typeparam name="TValue">The type of values generated.</typeparam>
	/// <typeparam name="TOptions">The options type used to configure the generator.</typeparam>
	public abstract class RandomBase<TValue, TOptions> where TOptions : IOptionsBase
	{
		/// <summary>
		/// The configured options for the random generator.
		/// </summary>
		protected readonly TOptions Options;

		/// <summary>
		/// Initializes a new instance of <see cref="RandomBase{TValue, TOptions}"/> with the specified options.
		/// </summary>
		/// <param name="options">The configuration options. Cannot be null.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
		protected RandomBase(TOptions options)
		{
			ArgumentNullException.ThrowIfNull(options);
			options.Validate();

			Options = options;
		}

		/// <summary>
		/// Generates the next random value.
		/// Must be implemented by derived random generator classes.
		/// </summary>
		/// <returns>A new random value of type <typeparamref name="TValue"/>.</returns>
		public abstract TValue Next();

		/// <summary>
		/// Generates the next random value, excluding any value for which <paramref name="excludeFn"/> returns true.
		/// </summary>
		/// <param name="excludeFn">A function that returns true for values to exclude. Cannot be null.</param>
		/// <returns>A random value that does not satisfy <paramref name="excludeFn"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="excludeFn"/> is null.</exception>
		public TValue NextExcept(Func<TValue, bool> excludeFn)
		{
			ArgumentNullException.ThrowIfNull(excludeFn);

			TValue next;

			do
			{
				next = Next();
			} while (excludeFn.Invoke(next));

			return next;
		}

		/// <summary>
		/// Generates the next random value, excluding a single specified value.
		/// </summary>
		/// <param name="excluded">The value to exclude from generation.</param>
		/// <returns>A random value not equal to <paramref name="excluded"/>.</returns>
		public TValue NextExcept(TValue excluded) => NextExcept([excluded]);

		/// <summary>
		/// Generates the next random value, excluding a set of specified values.
		/// </summary>
		/// <param name="excludedValues">A collection of values to exclude. Cannot be null.</param>
		/// <returns>A random value that is not contained in <paramref name="excludedValues"/>.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="excludedValues"/> is null.</exception>
		public virtual TValue NextExcept(IEnumerable<TValue> excludedValues)
		{
			ArgumentNullException.ThrowIfNull(excludedValues);

			var excludedSet = new HashSet<TValue>(excludedValues);
			if (excludedSet.Count == 0)
				return Next();

			TValue next;

			do
			{
				next = Next();
			} while (excludedSet.Contains(next));

			return next;
		}
	}
}
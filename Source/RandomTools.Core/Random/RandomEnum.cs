using RandomTools.Core.Options;

namespace RandomTools.Core.Random
{
	/// <summary>
	/// Generates random values from the specified enumeration <typeparamref name="TEnum"/>.
	/// </summary>
	/// <typeparam name="TEnum">The enumeration type. Must be a struct and an Enum.</typeparam>
	public sealed class RandomEnum<TEnum> : RandomBase<TEnum, NullOptions> where TEnum : struct, Enum
	{
		private readonly TEnum[] _values;

		/// <summary>
		/// Initializes a new instance of <see cref="RandomEnum{TEnum}"/>.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the enumeration has no defined values.</exception>
		public RandomEnum() : base(new NullOptions())
		{
			_values = Enum.GetValues<TEnum>();

			if (_values.Length == 0)
			{
				throw new ArgumentException($"Enumeration '{typeof(TEnum).Name}' must have at least one value.", nameof(TEnum));
			}
		}

		/// <summary>
		/// Returns a uniformly random value from the enumeration.
		/// </summary>
		public override TEnum Next() => _values[CoreTools.NextInt(0, _values.Length)];
	}
}

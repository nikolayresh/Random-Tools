using RandomTools.Core.Exceptions;

namespace RandomTools.Core.Options
{
	/// <summary>
	/// Represents configuration options for generating random boolean values.
	/// <para>
	/// Allows specifying an optional <see cref="Bias"/> to control the probability of returning <c>true</c> values.
	/// </para>
	/// <para>
	/// If <see cref="Bias"/> is <c>null</c>, random booleans are generated with equal probability <c>(50/50)</c>.  
	/// Otherwise, <see cref="Bias"/> determines the approximate percentage of <c>true</c> values over multiple calls 
	/// to <c>RandomBool.Next()</c>.
	/// </para>
	/// </summary>
	public sealed class BoolOptions : IOptionsBase, IEquatable<BoolOptions?>
	{
		/// <summary>
		/// Optional bias used to generate <c>true</c> for each random boolean value.
		/// <para>
		/// Valid range: <c>0.0 &lt; Bias &lt; 1.0</c> (exclusive).  
		/// If <c>Bias</c> is <c>null</c>, values are generated with equal probability <c>(50/50)</c>.  
		/// For example, if <c>Bias = 0.7</c>, approximately 70% of values will be <c>true</c> 
		/// and 30% will be <c>false</c> over multiple calls to <c>RandomBool.Next()</c>.
		/// </para>
		/// </summary>
		internal double? Bias;

		/// <summary>
		/// Sets the bias toward generating <c>true</c> values.
		/// </summary>
		/// <param name="bias">
		/// The bias value as a <see cref="double"/> in the exclusive range <c>(0.0 - 1.0)</c>.  
		/// Represents the probability that <c>RandomBool.Next()</c> returns <c>true</c> over multiple calls.
		/// </param>
		public BoolOptions WithBias(double bias)
		{
			Bias = bias;
			return this;
		}

		/// <summary>
		/// Validates the current configuration of this <see cref="BoolOptions"/> instance.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method ensures that the configured <see cref="Bias"/> value (if provided) falls within the
		/// valid exclusive range of <c>0.0 &lt; Bias &lt; 1.0</c>.
		/// </para>
		/// <para>
		/// A <see cref="Bias"/> less than or equal to <c>0.0</c> would prevent any <c>true</c> values from being generated,
		/// while a <see cref="Bias"/> greater than or equal to <c>1.0</c> would prevent any <c>false</c> values.
		/// To maintain randomness, both outcomes must remain possible.
		/// </para>
		/// <para>
		/// If <see cref="Bias"/> is <c>null</c>, the validation passes automatically, indicating equal probability (50/50).
		/// </para>
		/// </remarks>
		/// <exception cref="OptionsValidationException">
		/// Thrown when <see cref="Bias"/> is set to an invalid value:
		/// <list type="bullet">
		/// <item>
		/// <term><c>Bias ≤ 0.0</c></term>
		/// <description>No <c>true</c> values can be generated.</description>
		/// </item>
		/// <item>
		/// <term><c>Bias ≥ 1.0</c></term>
		/// <description>No <c>false</c> values can be generated.</description>
		/// </item>
		/// </list>
		/// </exception>
		public void Validate()
		{
			if (!Bias.HasValue)
				return;

			if (Bias <= 0.0)
			{
				throw new OptionsValidationException(this,
					$"Bias must be greater than 0.0 to allow 'true' values to be generated. Actual value: {Bias}");
			}

			if (Bias >= 1.0)
			{
				throw new OptionsValidationException(this,
					$"Bias must be less than 1.0 to allow 'false' values to be generated. Actual value: {Bias}");
			}
		}

		public IOptionsBase Clone() => new BoolOptions
		{
			Bias = Bias
		};

		public override bool Equals(object? obj) => Equals(obj as BoolOptions);

		public bool Equals(BoolOptions? other) =>
			other is not null &&
			other.Bias == Bias;

		public override int GetHashCode() => Bias?.GetHashCode() ?? 0;
	}
}

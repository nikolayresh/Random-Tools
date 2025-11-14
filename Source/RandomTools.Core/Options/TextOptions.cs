namespace RandomTools.Core.Options
{
	/// <summary>
	/// Represents options for configuring random text generation.
	/// Allows specifying character sets, length, and uniqueness.
	/// </summary>
	public sealed class TextOptions : IOptionsBase, IEquatable<TextOptions>
	{
		internal bool Digits;
		internal bool LowerLetters;
		internal bool UpperLetters;
		internal int Length;
		internal bool IsUnique;

		/// <summary>
		/// Sets the desired length of the generated text.
		/// </summary>
		public TextOptions UseLength(int length)
		{
			Length = length;
			return this;
		}

		/// <summary>
		/// Enables or disables the use of digits.
		/// </summary>
		public TextOptions UseDigits(bool include = true)
		{
			Digits = include;
			return this;
		}

		/// <summary>
		/// Enables or disables the use of lowercase letters.
		/// </summary>
		public TextOptions UseLowerLetters(bool value = true)
		{
			LowerLetters = value;
			return this;
		}

		/// <summary>
		/// Enables or disables the use of uppercase letters.
		/// </summary>
		public TextOptions UseUpperLetters(bool value = true)
		{
			UpperLetters = value;
			return this;
		}

		/// <summary>
		/// Specifies whether the generated text should have unique characters.
		/// </summary>
		public TextOptions Unique(bool value = true)
		{
			IsUnique = value; 
			return this;
		}

		public override bool Equals(object? obj) => Equals(obj as TextOptions);

		public void Validate()
		{
			if (!Digits && !LowerLetters && !UpperLetters)
			{
				throw new OptionsValidationException(
					"At least one character set must be enabled: digits, lowercase letters, or uppercase letters");

			}

			if (Length <= 0)
			{
				throw new OptionsValidationException(
				   $"Invalid length: {Length}. Length must be greater than zero.");
			}

			if (IsUnique)
			{
				var maxLength = (Digits ? 10 : 0) + (LowerLetters ? 26 : 0) + (UpperLetters ? 26 : 0);
				if (Length > maxLength)
				{
					throw new OptionsValidationException(
						$"Invalid configuration: requested length {Length} exceeds maximum unique characters {maxLength} based on selected sets");
				}
			}
		}

		public bool Equals(TextOptions? other) =>
			other is not null && 
			other.Digits == Digits && 
			other.LowerLetters == LowerLetters && 
			other.UpperLetters == UpperLetters && 
			other.Length == Length && 
			other.IsUnique == IsUnique;

		public override int GetHashCode() => 
			HashCode.Combine(Digits, LowerLetters, UpperLetters, Length, IsUnique);
	}
}

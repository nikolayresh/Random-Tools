using RandomTools.Core.Options;
using System.Security.Cryptography;

namespace RandomTools.Core.Random
{
	public class RandomText : RandomBase<string, TextOptions>
	{
		private static readonly string Digits = new([.. Enumerable.Range('0', 10).Select(x => (char)x)]);
		private static readonly string LowerLetters = new([.. Enumerable.Range('a', 26).Select(x => (char)x)]);
		private static readonly string UpperLetters = new([.. Enumerable.Range('A', 26).Select(x => (char)x)]);

		public RandomText(TextOptions options) : base(options)
		{
		}

		private string GenerateUniqueText(string characters)
		{
			var shuffled = characters.ToCharArray().AsSpan();
			RandomNumberGenerator.Shuffle(shuffled);

			return new string(shuffled[..Options.Length]);
		}

		private string JoinCharacters()
		{
			var characters = string.Empty;

			if (Options.Digits)
				characters += Digits;

			if (Options.LowerLetters)
				characters += LowerLetters;

			if (Options.UpperLetters)
				characters += UpperLetters;

			return characters;
		}

		public override string Next()
		{
			var characters = JoinCharacters();

			return Options.IsUnique
				? GenerateUniqueText(characters) 
				: GenerateText(characters);
		}

		private string GenerateText(string characters)
		{
			var buffer = new char[Options.Length];

			for (int i = 0; i < Options.Length; i++)
			{
				buffer[i] = characters[RandomNumberGenerator.GetInt32(characters.Length)];
			}

			return new string(buffer);
		}
	}
}

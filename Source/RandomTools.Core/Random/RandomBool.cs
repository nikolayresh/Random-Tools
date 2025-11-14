using RandomTools.Core.Options;

namespace RandomTools.Core.Random
{
	/// <summary>
	/// Generates random boolean (<see cref="bool"/>) values,
	/// optionally influenced by a configurable bias.
	/// </summary>
	/// <remarks>
	/// When a bias is specified in <see cref="BoolOptions.Bias"/>,
	/// the probability of returning <see langword="true"/> equals that bias value (0.0–1.0).
	/// If no bias is specified, values are generated with an equal 50/50 probability.
	/// </remarks>
	public class RandomBool : RandomBase<bool, BoolOptions>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RandomBool"/> class
		/// using the specified <see cref="BoolOptions"/> configuration.
		/// </summary>
		/// <param name="options">
		/// The options that define bias and other behavior for random boolean generation.
		/// </param>
		public RandomBool(BoolOptions options) : base(options) 
		{ 
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomBool"/> class 
		/// with default unbiased settings.
		/// </summary>
		/// <remarks>
		/// This constructor creates a generator with no bias,
		/// producing <see langword="true"/> and <see langword="false"/> values
		/// with equal probability (50/50).
		/// </remarks>
		public RandomBool() : base(new BoolOptions())
		{
		}

		/// <summary>
		/// Returns a random boolean value.
		/// </summary>
		/// <param name="excludedValues">
		/// A collection of boolean values to exclude from generation. 
		/// This method will only succeed if the collection is empty.
		/// </param>
		/// <returns>
		/// A random boolean value. 
		/// This method requires <paramref name="excludedValues"/> to be empty to maintain randomness.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="excludedValues"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="excludedValues"/> contains one or more values. 
		/// In that case, the method cannot produce a random result and could only return 
		/// the remaining boolean value, making the result entirely deterministic.
		/// </exception>
		public override bool NextExcept(IEnumerable<bool> excludedValues)
		{
			ArgumentNullException.ThrowIfNull(excludedValues);

			var excludedSet = new HashSet<bool>(excludedValues);
			if (excludedSet.Count >= 1)
			{
				throw new ArgumentException(
					"Excluding one or more boolean values ('true' or 'false') is not allowed. " +
					"If the method were to execute with exclusions, it could only return the remaining boolean value, " +
					"making the result entirely deterministic rather than random.",
					nameof(excludedValues));
			}

			return Next();
		}

		/// <summary>
		/// Returns the next random boolean value.
		/// </summary>
		/// <remarks>
		/// If <see cref="BoolOptions.Bias"/> is set, the probability of returning <see langword="true"/> 
		/// equals the specified bias. Otherwise, a fair 50/50 coin flip is performed.
		/// </remarks>
		/// <returns>A random boolean value, optionally influenced by the configured bias</returns>
		public override bool Next()
		{
			// Use bias if specified
			if (Options.Bias is double bias)
			{
				// Generate a random double in [0,1) and compare it to bias
				double randomValue = CoreTools.NextDouble();
				return randomValue < bias;
			}

			// Pure 50/50 coin flip
			byte randomByte = CoreTools.NextByte();
			return (randomByte & 1) == 0;
		}
	}
}

namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Container for all delay option types.
	/// </summary>
	public static class DelayOptions
	{
		/// <summary>
		/// Represents a uniform delay distribution where all values in the range [Minimum, Maximum] 
		/// are equally likely.
		/// </summary>
		public class Uniform : DelayOptionsBase<Uniform>
		{
			// Currently no additional fields; inherits Minimum, Maximum, and TimeUnit from DelayOptionsBase
		}

		/// <summary>
		/// Represents a normal (Gaussian) delay distribution.
		/// Users can optionally specify Mean and StandardDeviation.
		/// If not specified, defaults will be used:
		/// - Mean = midpoint of [Minimum, Maximum]
		/// - StandardDeviation = 1/6 of the range, so ~99.7% values fall within [Minimum, Maximum]
		/// </summary>
		public sealed class Normal : DelayOptionsBase<Normal>
		{
			/// <summary>
			/// Optional mean (center) of the normal distribution.
			/// If null, the midpoint of [Minimum, Maximum] is used.
			/// </summary>
			internal double? Mean;

			/// Optional standard deviation (spread) of the normal distribution.
			/// If null, defaults to 1/6 of the range [Minimum, Maximum].
			/// Must be positive if specified.
			/// </summary>
			internal double? StandardDeviation;

			/// <summary>
			/// Sets the mean (center) of the normal distribution.
			/// </summary>
			/// <param name="value">Mean value in the same units as Minimum/Maximum.</param>
			/// <returns>The current instance for fluent configuration.</returns>
			public Normal WithMean(double value)
			{
				Mean = value;
				return this;
			}

			/// <summary>
			/// Sets the standard deviation (spread) of the normal distribution.
			/// </summary>
			/// <param name="value">Standard deviation in the same units as Minimum/Maximum. Must be positive.</param>
			/// <returns>The current instance for fluent configuration.</returns>
			public Normal WithStandardDeviation(double value) 
			{
				StandardDeviation = value; 
				return this; 
			}

			/// <summary>
			/// Validates the options to ensure:
			/// - Mean (if specified) is within [Minimum, Maximum] (flexible, can equal the boundaries)
			/// - StandardDeviation (if specified) is positive
			/// </summary>
			public override void Validate()
			{
				base.Validate();

				if (Mean.HasValue && (Mean.Value < Minimum || Mean.Value > Maximum))
				{
					throw new OptionsValidationException(
						$"Mean ({Mean.Value}) must be within the range [{Minimum} - {Maximum}].");
				}

				if (StandardDeviation.HasValue && StandardDeviation.Value <= 0)
				{
					throw new OptionsValidationException(
						$"StandardDeviation ({StandardDeviation.Value}) must be positive.");
				}
			}
		}
	}
}
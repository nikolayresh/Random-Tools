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
		public sealed class Uniform : DelayOptionsBase<Uniform>
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

			internal double GetEffectiveMean()
			{
				if (Mean.HasValue)
				{
					return Mean.Value;
				}

				double value = (Minimum + Maximum) / 2.0;
				return value;
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

		public sealed class Exponential : DelayOptionsBase<Exponential>
		{
			/// <summary>
			/// Optional rate parameter λ of the exponential distribution.
			/// Must be positive if specified.
			/// If null, a default λ is computed so that the mean is equal to the midpoint of [Minimum, Maximum].
			/// Mean = 1 / λ.
			/// </summary>
			internal double? Lambda;

			/// <summary>
			/// Sets the rate parameter λ (lambda) for the exponential distribution.
			/// </summary>
			/// <param name="value">Rate parameter. Must be positive.</param>
			/// <returns>The current instance for fluent configuration.</returns>
			public Exponential WithLambda(double value)
			{
				Lambda = value;
				return this;
			}

			/// <summary>
			/// Computes the effective rate parameter λ to be used.
			/// If the user specified λ explicitly, it is returned.
			/// Otherwise, a default value corresponding to the midpoint mean is used.
			/// </summary>
			internal double GetEffectiveLambda()
			{
				if (Lambda.HasValue)
					return Lambda.Value;

				// Default: mean = midpoint => lambda = 1 / mean
				double mean = (Minimum + Maximum) / 2.0;

				// Safety: avoid division by zero if the range is degenerate
				if (mean <= 0)
					throw new OptionsValidationException(
						"Cannot compute default lambda: midpoint mean is zero or negative.");

				return 1.0 / mean;
			}

			/// <summary>
			/// Validates:
			/// - Lambda (if specified) must be positive.
			/// - Minimum/Maximum are validated by base class.
			/// </summary>
			public override void Validate()
			{
				base.Validate();

				if (Lambda.HasValue && Lambda.Value <= 0)
				{
					throw new OptionsValidationException(
						$"Lambda ({Lambda.Value}) must be positive.");
				}

				// No further constraints: exponential is unbounded above by definition
			}
		}

	}
}
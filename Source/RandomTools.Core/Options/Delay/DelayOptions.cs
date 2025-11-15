using RandomTools.Core.Exceptions;

namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Contains configuration types for different delay-distribution strategies.
	/// </summary>
	public static class DelayOptions
	{
		/// <summary>
		/// Configuration for a uniform delay distribution.
		/// Every value in the interval [Minimum, Maximum] is equally probable.
		/// </summary>
		public sealed class Uniform : DelayOptionsBase<Uniform>
		{
			// No additional configuration for now.
			// Inherits Minimum, Maximum, and TimeUnit from DelayOptionsBase.
		}

		/// <summary>
		/// Configuration for a normal (Gaussian) delay distribution.
		/// 
		/// Optional parameters:
		/// - Mean: if omitted, defaults to the midpoint of [Minimum, Maximum].
		/// - StandardDeviation: if omitted, defaults to one-sixth of the range,
		///   resulting in ~99.7% of values falling within the bounds.
		/// </summary>
		public sealed class Normal : DelayOptionsBase<Normal>
		{
			/// <summary>
			/// Optional mean (μ) of the normal distribution.
			/// If zero, the midpoint of the delay range will be used.
			/// </summary>
			internal double Mean;

			/// <summary>
			/// Optional standard deviation (σ).
			/// If zero, a default value of (Maximum − Minimum) / 6 is used.
			/// Must be strictly positive when explicitly set.
			/// </summary>
			internal double StandardDeviation;

			/// <summary>
			/// Sets the mean (μ) of the normal distribution.
			/// </summary>
			/// <param name="value">Mean value in the same units as Minimum and Maximum.</param>
			public Normal WithMean(double value)
			{
				Mean = value;
				return this;
			}

			/// <summary>
			/// Sets the standard deviation (σ) of the normal distribution.
			/// </summary>
			/// <param name="value">Standard deviation. Must be strictly positive.</param>
			public Normal WithStandardDeviation(double value)
			{
				StandardDeviation = value;
				return this;
			}

			/// <summary>
			/// Validates the configuration:
			/// - Ensures Minimum and Maximum are valid (base validation).
			/// - Ensures StandardDeviation, when explicitly set, is > 0.
			/// 
			/// Notes:
			/// Mean is not required to lie within the range because the distribution
			/// is later truncated (clamped) by the normal generator.
			/// </summary>
			public override void Validate()
			{
				base.Validate();

				if (StandardDeviation <= 0.0)
				{
					throw new OptionsValidationException(this,
						$"StandardDeviation ({StandardDeviation}) must be positive.");
				}
			}
		}

		/// <summary>
		/// Configuration for an exponential delay distribution.
		/// 
		/// Optional parameter:
		/// - Lambda (λ): the rate parameter.
		///   If not specified, λ is computed such that the mean (1 / λ)
		///   equals the midpoint of the interval [Minimum, Maximum].
		/// </summary>
		public sealed class Exponential : DelayOptionsBase<Exponential>
		{
			/// <summary>
			/// Optional rate parameter λ.
			/// Must be strictly positive when explicitly set.
			/// If null, λ is derived automatically from the midpoint of the range.
			/// </summary>
			internal double? Lambda;

			/// <summary>
			/// Sets the exponential rate parameter λ.
			/// </summary>
			/// <param name="value">Rate parameter. Must be > 0.</param>
			public Exponential WithLambda(double value)
			{
				Lambda = value;
				return this;
			}

			/// <summary>
			/// Resolves the effective λ to use:
			/// - Returns explicitly set λ, or
			/// - Computes a default λ based on mean = midpoint, λ = 1 / mean.
			/// </summary>
			internal double GetEffectiveLambda()
			{
				if (Lambda.HasValue)
					return Lambda.Value;

				double mean = (Minimum + Maximum) / 2.0;

				if (mean <= 0)
					throw new OptionsValidationException(this,
						"Cannot compute default lambda: midpoint mean is zero or negative.");

				return 1.0 / mean;
			}

			/// <summary>
			/// Validates configuration:
			/// - Minimum/Maximum validated by the base class.
			/// - Lambda must be > 0 when explicitly set.
			/// </summary>
			public override void Validate()
			{
				base.Validate();

				if (Lambda.HasValue && Lambda.Value <= 0)
				{
					throw new OptionsValidationException(this,
						$"Lambda ({Lambda.Value}) must be positive.");
				}

				// Note: Exponential distribution is unbounded above,
				// so no constraints related to [Minimum, Maximum].
			}
		}
	}
}

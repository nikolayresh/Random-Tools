using RandomTools.Core.Exceptions;

namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Serves purely as a container for delay-distribution option types.
	/// </summary>
	public static class DelayOptions
	{
		/// <summary>
		/// Configuration for a uniform delay distribution.
		/// <para>
		/// Every value in the interval [Minimum, Maximum] is equally likely.
		/// This distribution is simple, efficient, and does not use rejection sampling.
		/// </para>
		/// </summary>
		public sealed class Uniform : DelayOptionsBase<Uniform>
		{
			/// <summary>
			/// Validates configuration for the uniform distribution.
			/// <para>
			/// Validation rules:
			/// <list type="bullet">
			/// <item>
			/// <description>
			/// Base validation ensures that Minimum &lt; Maximum and the time unit is valid.
			/// </description>
			/// </item>
			/// <item>
			/// <description>
			/// The uniform distribution requires a non-zero interval width.  
			/// If the configured range collapses to a single floating-point value,
			/// the generator cannot produce meaningful randomness.
			/// </description>
			/// </item>
			/// </list>
			/// </para>
			/// </summary>
			/// <exception cref="OptionsValidationException">
			/// Thrown when the usable interval width is too small to generate
			/// uniformly distributed values.
			/// </exception>
			public override void Validate()
			{
				base.Validate();

				if ((Maximum - Minimum) <= double.Epsilon)
				{
					throw new OptionsValidationException(this,
						$"Invalid uniform distribution range: [{Minimum}, {Maximum}]. " +
						$"The interval is too narrow to produce meaningful randomness. " +
						$"Increase the distance between [Minimum] and [Maximum] to allow reliable uniform sampling.");
				}
			}
		}

		/// <summary>
		/// Configuration options for a normal (Gaussian) delay distribution.
		/// <para>
		/// Mean (μ) and StandardDeviation (σ) describe the underlying normal distribution.
		/// Generated delays are truncated to the range [Minimum, Maximum].
		/// </para>
		/// </summary>
		public sealed class Normal : DelayOptionsBase<Normal>
		{
			/// <summary>
			/// Mean (μ) of the underlying normal distribution.
			/// This value is NOT clamped to the allowed output range and may lie outside it,
			/// because the actual generator performs truncation at runtime.
			/// </summary>
			internal double Mean;

			/// <summary>
			/// Standard deviation (σ) of the underlying normal distribution.
			/// Must be strictly positive.
			/// </summary>
			internal double StandardDeviation;

			/// <summary>
			/// Sets the mean (μ) of the distribution.
			/// </summary>
			public Normal WithMean(double value)
			{
				Mean = value; 
				return this;
			}

			/// <summary>
			/// Sets the standard deviation (σ).
			/// Value must be greater than zero.
			/// </summary>
			public Normal WithStandardDeviation(double value)
			{
				StandardDeviation = value;
				return this;
			}

			/// <summary>
			/// Sets the minimum and maximum bounds and automatically derives
			/// a reasonable mean and standard deviation:
			/// <para>
			/// • Mean = midpoint of the range  
			/// • σ = (max − min) / 6, meaning ±3σ spans the full interval  
			///   (~99.7% of the distribution under the 6σ rule)
			/// </para>
			/// </summary>
			public Normal WithAutoFit(double minimum, double maximum)
			{
				WithMinimum(minimum);
				WithMaximum(maximum);

				Mean = (minimum + maximum) / 2.0;
				StandardDeviation = (maximum - minimum) / 6.0; 

				return this;
			}

			/// <summary>
			/// Validates configuration:
			/// <para>
			/// • Base class ensures Minimum &lt; Maximum and correct TimeUnit  
			/// • σ must be strictly positive  
			/// • The distribution must have a non-negligible probability of producing
			///   values inside the configured range
			/// </para>
			/// </summary>
			public override void Validate()
			{
				base.Validate();

				EnsureFinite(Mean);
				EnsureFinite(StandardDeviation);

				if (StandardDeviation <= double.Epsilon)
				{
					throw new OptionsValidationException(this,
						$"Standard deviation ({StandardDeviation}) must be a positive numeric value. " +
						$"Zero or negative σ prevents meaningful generation of delays.");
				}

				if ((Maximum - Minimum) <= double.Epsilon)
				{
					throw new OptionsValidationException(this,
						$"Configured range [{Minimum},{Maximum}] is too narrow. " +
						$"A normal distribution cannot reliably generate values within such a small interval.");
				}

				double pRange = GaussianTools.GetRangeHitProbability(Mean, StandardDeviation, (Minimum, Maximum));
				if (pRange <= double.Epsilon)
				{
					throw new OptionsValidationException(this,
						$"Normal distribution (μ={Mean}, σ={StandardDeviation}) almost never produces values " +
						$"within the range [{Minimum},{Maximum}]. Consider adjusting μ, σ, or the range.");
				}
			}

			public override bool Equals(object? obj)
			{
				if (!base.Equals(obj))
					return false;

				if (obj is not Normal other)
					return false;

				return
					other.Mean == Mean &&
					other.StandardDeviation == StandardDeviation;
			}

			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Mean, StandardDeviation);
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


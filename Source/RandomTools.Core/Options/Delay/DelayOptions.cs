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
			/// <list type="bullet">
			///   <item>
			///     <description>Mean (μ) = midpoint of the range</description>
			///   </item>
			///   <item>
			///     <description>
			///       Standard deviation (σ) = (maximum − minimum) / 6, meaning that ±3σ
			///       spans the full interval (~99.7% of the distribution under the 6σ rule)
			///     </description>
			///   </item>
			/// </list>
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

			public override bool Equals(Normal? other)
			{
				return base.Equals(other) &&
					other.Mean == Mean &&
					other.StandardDeviation == StandardDeviation;
			}

			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Mean, StandardDeviation);
		}

		public sealed class Triangular : DelayOptionsBase<Triangular>
		{
			internal double Mode;

			public Triangular WithMode(double value)
			{
				Mode = value;
				return this;
			}

			public override void Validate()
			{
				base.Validate();
				EnsureFinite(Mode);

				if (Mode < Minimum || Mode > Maximum)
				{
					throw new OptionsValidationException(this,
						$"Mode ({Mode}) must lie within the range [{Minimum}, {Maximum}]");
				}
			}

			public override bool Equals(Triangular? other)
			{
				return base.Equals(other) &&
					other.Mode == Mode;
			}

			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, Mode, TimeUnit);
		}

		/// <summary>
		/// Configuration options for an arcsine delay distribution.
		/// Ensures that the interval [Minimum, Maximum] is valid for meaningful sampling.
		/// </summary>
		public sealed class Arcsine : DelayOptionsBase<Arcsine>
		{
			/// <summary>
			/// Validates that the configured range is wide enough to produce meaningful randomness.
			/// </summary>
			/// <exception cref="OptionsValidationException">
			/// Thrown when <see cref="Maximum"/> and <see cref="Minimum"/> are too close.
			/// </exception>
			public override void Validate()
			{
				base.Validate();

				if ((Maximum - Minimum) <= double.Epsilon)
				{
					throw new OptionsValidationException(this,
						$@"Invalid arcsine distribution range: [{Minimum}, {Maximum}]. 
The interval is too narrow to produce meaningful randomness. 
Increase the distance between [Minimum] and [Maximum] to allow reliable arcsine sampling.");
				}
			}
		}
	}
}
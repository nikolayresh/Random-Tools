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

				double pRange = GaussianTools.GetRangeHitProbability(Mean, StandardDeviation, (Minimum, Maximum));
				if (pRange <= double.Epsilon)
				{
					throw new OptionsValidationException(this,
						$"Normal distribution (μ={Mean}, σ={StandardDeviation}) almost never produces values " +
						$"within the range [{Minimum},{Maximum}]. Consider adjusting μ, σ, or the range.");
				}
			}

			public override bool Equals(Normal? other) => 
				base.Equals(other) &&
				DoubleComparer.Equals(other.Mean, Mean) &&
				DoubleComparer.Equals(other.StandardDeviation, StandardDeviation);

			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Mean, StandardDeviation);
		}

		/// <summary>
		/// Options for generating a Triangular distribution-based delay.
		/// </summary>
		public sealed class Triangular : DelayOptionsBase<Triangular>
		{
			/// <summary>
			/// The mode (peak) of the triangular distribution.
			/// Must be between <see cref="Minimum"/> and <see cref="Maximum"/>.
			/// </summary>
			internal double Mode;

			/// <summary>
			/// Sets the mode of the triangular distribution.
			/// </summary>
			/// <param name="value">The mode value.</param>
			/// <returns>The current <see cref="Triangular"/> instance for fluent configuration.</returns>
			public Triangular WithMode(double value)
			{
				Mode = value;
				return this;
			}

			/// <summary>
			/// Validates the options to ensure they are consistent and usable.
			/// </summary>
			public override void Validate()
			{
				base.Validate();
				EnsureFinite(Mode);

				// Check that the mode lies within the defined [Minimum, Maximum] range
				if (Mode < Minimum || Mode > Maximum)
				{
					throw new OptionsValidationException(this,
						$"Mode ({Mode}) must lie within the range [{Minimum}, {Maximum}]"
					);
				}
			}

			/// <summary>
			/// Determines whether the current instance is equal to another <see cref="Triangular"/> instance.
			/// </summary>
			/// <param name="other">Another <see cref="Triangular"/> instance.</param>
			/// <returns>True if equal, false otherwise.</returns>
			public override bool Equals(Triangular? other) =>
				base.Equals(other) &&
				DoubleComparer.Equals(other.Mode, Mode);

			/// <summary>
			/// Returns a hash code for the current instance.
			/// </summary>
			/// <returns>A combined hash code of the relevant properties.</returns>
			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Mode);
		}

		/// <summary>
		/// Configuration options for an arcsine delay distribution.
		/// Ensures that the interval [Minimum, Maximum] is valid for meaningful sampling.
		/// </summary>
		public sealed class Arcsine : DelayOptionsBase<Arcsine>
		{
		}

		/// <summary>
		/// Configuration options for a <b>Bates distribution</b> used to generate
		/// bounded random delays.
		/// <para>
		/// The Bates distribution is defined as the arithmetic mean of
		/// <c>N</c> independent uniform samples. It produces a smooth,
		/// bell-shaped distribution that remains strictly within the
		/// configured <see cref="Minimum"/> and <see cref="Maximum"/> range.
		/// </para>
		/// <para>
		/// When <c>Samples = 1</c>, the distribution degenerates to a uniform
		/// distribution. Increasing <c>Samples</c> makes the distribution
		/// progressively smoother and more concentrated toward the center,
		/// approaching a bounded normal-like shape.
		/// </para>
		/// </summary>
		public sealed class Bates : DelayOptionsBase<Bates>
		{
			/// <summary>
			/// Gets or sets the number of uniform samples used to construct the
			/// Bates distribution. Must be at least <c>1</c>.
			/// </summary>
			internal int Samples;

			/// <summary>
			/// Sets the number of uniform samples used to compute the Bates mean.
			/// Larger values produce smoother, more bell-shaped distributions.
			/// </summary>
			/// <param name="value">The number of samples (must be ≥ 1).</param>
			/// <returns>The current instance for fluent configuration.</returns>
			public Bates WithSamples(int value)
			{
				Samples = value;
				return this;
			}

			/// <summary>
			/// Validates the configuration and throws an <see cref="OptionsValidationException"/>
			/// if the settings are not suitable for generating a Bates distribution.
			/// </summary>
			public override void Validate()
			{
				base.Validate();

				if (Samples < 1)
				{
					throw new OptionsValidationException(this,
						$"Samples ({Samples}) must be at least 1 to produce meaningful Bates distribution sampling."
					);
				}
			}

			/// <inheritdoc />
			public override bool Equals(Bates? other) => 
				base.Equals(other) &&
			    other.Samples == Samples;

			/// <inheritdoc />
			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Samples);
		}

		/// <summary>
		/// Configuration options for a Polynomial / Power delay distribution.
		/// <para>
		/// The Polynomial distribution generates values in [Minimum, Maximum] with a density
		/// proportional to (x - Minimum)^Power or (Maximum - x)^Power if reversed.
		/// </para>
		/// <para>
		/// A Power of 0 produces a uniform distribution. Higher Power values produce
		/// increasingly skewed distributions toward Maximum (or Minimum if Reverse=true).
		/// </para>
		/// </summary>
		public sealed class Polynomial : DelayOptionsBase<Polynomial>
		{
			/// <summary>
			/// Gets the power exponent for the polynomial distribution.
			/// Must be >= 0.
			/// </summary>
			internal double Power;

			/// <summary>
			/// Indicates whether the distribution is reversed: density proportional to (Maximum - x)^Power.
			/// </summary>
			internal bool Reverse;

			/// <summary>
			/// Sets the power exponent (Power ≥ 0) for the polynomial distribution.
			/// </summary>
			/// <param name="value">Exponent value.</param>
			/// <returns>The current instance for fluent configuration.</returns>
			public Polynomial WithPower(double value)
			{
				Power = value;
				return this;
			}

			/// <summary>
			/// Sets whether the distribution is reversed (more values near Minimum).
			/// </summary>
			/// <param name="value">True to reverse, false for normal orientation.</param>
			/// <returns>The current instance for fluent configuration.</returns>
			public Polynomial WithReverse(bool value)
			{
				Reverse = value;
				return this;
			}

			/// <summary>
			/// Validates the configuration, throwing an <see cref="OptionsValidationException"/>
			/// if any option is invalid.
			/// </summary>
			public override void Validate()
			{
				base.Validate();
				EnsureFinite(Power);

				if (Power < 0.0)
				{
					throw new OptionsValidationException(this,
						$"Power ({Power}) must be >= 0.");
				}
			}

			/// <inheritdoc/>
			public override bool Equals(Polynomial? other) =>
				base.Equals(other) &&
				DoubleComparer.Equals(other.Power, Power) &&
				other.Reverse == Reverse;

			/// <inheritdoc/>
			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Power, Reverse);
		}

		/// <summary>
		/// Represents configuration for generating delays based on a Beta distribution.
		/// </summary>
		/// <remarks>
		/// The <see cref="Beta"/> class allows configuring:
		/// - Minimum and Maximum delay range,
		/// - Time unit (e.g., milliseconds, seconds),
		/// - Alpha (α) and Beta (β) parameters of the Beta distribution.
		///
		/// The class validates all settings to ensure they are suitable for generating meaningful random delays.
		/// Methods such as <see cref="WithAlpha(double)"/> and <see cref="WithBeta(double)"/> follow a fluent API
		/// pattern and return the instance for chaining. 
		/// </remarks>
		public sealed class Beta : DelayOptionsBase<Beta>
		{
			/// <summary>
			/// Internal alpha parameter (α) for the Beta distribution. Must be positive.
			/// </summary>
			internal double AlphaValue;

			/// <summary>
			/// Internal beta parameter (β) for the Beta distribution. Must be positive.
			/// </summary>
			internal double BetaValue;

			/// <summary>
			/// Sets the alpha parameter (α) and returns the current instance for fluent configuration.
			/// </summary>
			/// <param name="value">Alpha value. Must be positive.</param>
			/// <returns>The current <see cref="Beta"/> instance with updated alpha.</returns>
			public Beta WithAlpha(double value)
			{
				AlphaValue = value;
				return this;
			}

			/// <summary>
			/// Sets the beta parameter (β) and returns the current instance for fluent configuration.
			/// </summary>
			/// <param name="value">Beta value. Must be positive.</param>
			/// <returns>The current <see cref="Beta"/> instance with updated beta.</returns>
			public Beta WithBeta(double value)
			{
				BetaValue = value;
				return this;
			}

			/// <summary>
			/// Validates the configuration for logical consistency and usability.
			/// </summary>
			/// <exception cref="OptionsValidationException">
			/// Thrown if any of the following conditions are violated:
			/// - <see cref="Minimum"/> or <see cref="Maximum"/> are non-finite,
			/// - <see cref="Minimum"/> > <see cref="Maximum"/>,
			/// - Range length (<c>Maximum - Minimum</c>) is too small to generate meaningful randomness,
			/// - <see cref="AlphaValue"/> ≤ 0 or <see cref="BetaValue"/> ≤ 0.
			/// </exception>
			public override void Validate()
			{
				// Validate base numeric fields (Minimum/Maximum)
				base.Validate();
				EnsureFinite(AlphaValue);
				EnsureFinite(BetaValue);

				if (AlphaValue <= 0.0)
				{
					throw new OptionsValidationException(this,
						$"Alpha (α) must be a positive numeric value, but was {AlphaValue}");
				}

				if (BetaValue <= 0.0)
				{
					throw new OptionsValidationException(this,
						$"Beta (β) must be a positive numeric value, but was {BetaValue}");
				}
			}

			/// <summary>
			/// Determines equality with another <see cref="Beta"/> instance.
			/// </summary>
			/// <param name="other">Other <see cref="Beta"/> instance to compare.</param>
			/// <returns><see langword="true"/> if all relevant fields are equal; otherwise <see langword="false"/>.</returns>
			public override bool Equals(Beta? other) => 
				base.Equals(other) &&
				DoubleComparer.Equals(other.AlphaValue, AlphaValue) &&
				DoubleComparer.Equals(other.BetaValue, BetaValue);

			/// <summary>
			/// Computes a hash code based on range, time unit, and Beta distribution parameters.
			/// </summary>
			/// <returns>A combined hash code.</returns>
			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, AlphaValue, BetaValue);
		}

		public sealed class Sequence : DelayOptionsBase<Sequence>
		{
			internal IList<double> Values = [];
			internal bool RandomizeOrder;

			public Sequence WithValues(params double[] values)
			{
				Values ??= [];
				foreach (double next in values)
				{
					Values.Add(next);
				}

				return this;
			}

			public Sequence WithValue(double value)
			{
				Values ??= [];
				Values.Add(value);

				return this;
			}

			public Sequence WithRandomizeOrder(bool value)
			{
				RandomizeOrder = value;
				return this;
			}

			public override void Validate()
			{
				base.Validate();

				if (Values is null)
				{
					throw new OptionsValidationException(this,
						$"Values array for the sequence distribution cannot be null.");
				}

				if (Values.Count == 0)
				{
					throw new OptionsValidationException(this,
						$"At least one value must be provided for the sequence distribution.");
				}

				foreach (double next in Values)
				{
					EnsureFinite(next);

					if (next < 0.0 || next > 1.0)
					{
						throw new OptionsValidationException(this,
							$"All sequence values must lie within [0.0, 1.0]. Invalid value: {next}");
					}
				}
			}

			public override bool Equals(Sequence? other) => 
				base.Equals(other) &&
				other.Values.SequenceEqual(Values);

			public override int GetHashCode() =>
				HashCode.Combine(Minimum, Maximum, TimeUnit, Values);
		}
	}
}
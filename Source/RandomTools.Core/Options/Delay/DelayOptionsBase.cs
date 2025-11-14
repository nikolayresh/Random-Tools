namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Base class for configuring delay options.
	/// Provides a range [Minimum, Maximum] and a <see cref="TimeUnit"/> for generating delays.
	/// This class is generic to support fluent configuration in derived types.
	/// </summary>
	/// <typeparam name="TDelayOptions">The concrete type of the delay options for fluent interface.</typeparam>
	public abstract class DelayOptionsBase<TDelayOptions> : IOptionsBase where TDelayOptions : DelayOptionsBase<TDelayOptions>
	{
		/// <summary>
		/// Minimum value of the delay range (inclusive).
		/// Must be positive and less than <see cref="Maximum"/>.
		/// </summary>
		internal double Minimum;

		/// <summary>
		/// Maximum value of the delay range (inclusive).
		/// Must be positive and greater than <see cref="Minimum"/>.
		/// </summary>
		internal double Maximum;

		/// <summary>
		/// The unit of time for the delay values (e.g., milliseconds, seconds, minutes).
		/// </summary>
		internal TimeUnit TimeUnit;

		/// <summary>
		/// Sets the minimum value for the delay range.
		/// </summary>
		/// <param name="value">The minimum delay value.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMinimum(double value)
		{
			Minimum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the maximum value for the delay range.
		/// </summary>
		/// <param name="value">The maximum delay value.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMaximum(double value)
		{
			Maximum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the time unit for the delay values.
		/// </summary>
		/// <param name="value">The <see cref="TimeUnit"/> to use.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithTimeUnit(TimeUnit value)
		{
			TimeUnit = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Validates the delay options to ensure logical and positive range values.
		/// Can be overridden in derived classes for additional validation logic.
		/// </summary>
		/// <exception cref="OptionsValidationException">
		/// Thrown if <see cref="Minimum"/> or <see cref="Maximum"/> are invalid.
		/// </exception>
		public virtual void Validate()
		{
			if (Minimum < 0)
			{
				throw new OptionsValidationException(
					$"Invalid {nameof(Minimum)} value: {Minimum}. It must be a non-negative integer.");
			}

			if (Maximum <= 0)
			{
				throw new OptionsValidationException(
					$"Invalid {nameof(Maximum)} value: {Maximum}. It must be a positive integer.");
			}

			if (Minimum >= Maximum)
			{
				throw new OptionsValidationException(
					$"Invalid range: {nameof(Minimum)} ({Minimum}) must be less than {nameof(Maximum)} ({Maximum}).");
			}
		}

		public override bool Equals(object? obj)
		{
			if (obj is null || obj.GetType() != GetType())
				return false;

			var other = (TDelayOptions)obj;

			// compare all fields that define equality
			return Minimum == other.Minimum &&
				   Maximum == other.Maximum &&
				   TimeUnit == other.TimeUnit;
		}

		public override int GetHashCode() => HashCode.Combine(Minimum, Maximum, TimeUnit);
	}
}
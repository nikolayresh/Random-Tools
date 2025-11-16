using RandomTools.Core.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Base class for configuring delay ranges expressed in a numeric interval
	/// and a <see cref="TimeUnit"/>. This class supports a fluent configuration
	/// pattern through its generic type parameter, allowing derived types to
	/// return their own type from configuration methods.
	/// </summary>
	/// <remarks>
	/// A delay range is defined by a closed interval: <c>[Minimum, Maximum]</c>.
	/// The only invalid numeric conditions are:
	/// <list type="bullet">
	/// <item><description><c>Minimum &lt; 0</c></description></item>
	/// <item><description><c>Maximum &lt; 0</c></description></item>
	/// <item><description><c>Minimum &gt; Maximum</c></description></item>
	/// </list>
	/// </remarks>
	/// <typeparam name="TDelayOptions">
	/// The concrete delay-options type used to enable a fluent API.
	/// </typeparam>
	public abstract class DelayOptionsBase<TDelayOptions> : IOptionsBase where TDelayOptions : DelayOptionsBase<TDelayOptions>
	{
		/// <summary>
		/// Gets or sets the minimum delay value of the range.
		/// Must be non-negative and must not exceed <see cref="Maximum"/>.
		/// </summary>
		internal double Minimum;

		/// <summary>
		/// Gets or sets the maximum delay value of the range.
		/// Must be non-negative.
		/// </summary>
		internal double Maximum;

		/// <summary>
		/// Specifies the time unit (e.g., milliseconds, seconds, minutes)
		/// applied to both <see cref="Minimum"/> and <see cref="Maximum"/>.
		/// </summary>
		internal TimeUnit TimeUnit;

		/// <summary>
		/// Sets the minimum delay value.
		/// </summary>
		/// <param name="value">The minimum delay value. Must be non-negative.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMinimum(double value)
		{
			Minimum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the maximum delay value.
		/// </summary>
		/// <param name="value">The maximum delay value. Must be non-negative.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMaximum(double value)
		{
			Maximum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the time unit used to interpret delay values.
		/// </summary>
		/// <param name="value">The time unit.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithTimeUnit(TimeUnit value)
		{
			TimeUnit = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Validates the configured delay interval to ensure it is logically consistent
		/// for time-based delay generation.
		/// </summary>
		/// <exception cref="OptionsValidationException">
		/// Thrown if any of the following rules are violated:
		/// <list type="bullet">
		/// <item><description><c>Minimum</c> and <c>Maximum</c> must be finite, non-NaN values.</description></item>
		/// <item><description><c>Minimum</c> must be non-negative.</description></item>
		/// <item><description><c>Maximum</c> must be non-negative.</description></item>
		/// <item><description><c>Minimum</c> must not exceed <c>Maximum</c>.</description></item>
		/// </list>
		/// </exception>
		public virtual void Validate()
		{
			if (!double.IsFinite(Minimum))
			{
				throw new OptionsValidationException(this,
					$"{nameof(Minimum)} ({Minimum}) must be a finite numeric value.");
			}

			if (!double.IsFinite(Maximum))
			{
				throw new OptionsValidationException(this,
					$"{nameof(Maximum)} ({Maximum}) must be a finite numeric value.");
			}

			if (Minimum > Maximum)
			{
				throw new OptionsValidationException(this,
					$"{nameof(Minimum)} ({Minimum}) cannot be greater than {nameof(Maximum)} ({Maximum}).");
			}
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current instance, 
		/// checking <see cref="Minimum"/>, <see cref="Maximum"/>, and <see cref="TimeUnit"/>.
		/// </summary>
		/// <inheritdoc />
		public override bool Equals([NotNullWhen(true)] object? obj)
		{
			// Use pattern matching for type check and conversion
			if (obj is not TDelayOptions other)
			{
				return false;
			}

			// The check for obj.GetType() != typeof(TDelayOptions) is handled implicitly
			// by the pattern match unless the object is a derived type that also
			// matches the pattern, which TDelayOptions is supposed to prevent anyway.

			return other.Minimum == Minimum &&
				   other.Maximum == Maximum &&
				   other.TimeUnit == TimeUnit;
		}

		/// <summary>
		/// Returns a hash code for the current instance.
		/// </summary>
		/// <inheritdoc />
		public override int GetHashCode() =>
			HashCode.Combine(Minimum, Maximum, TimeUnit);
	}
}
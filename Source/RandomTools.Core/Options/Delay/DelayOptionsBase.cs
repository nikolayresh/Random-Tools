using RandomTools.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
	public abstract class DelayOptionsBase<TDelayOptions> : IOptionsBase, IEquatable<TDelayOptions> where TDelayOptions : DelayOptionsBase<TDelayOptions>
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
			EnsureFinite(Minimum);
			EnsureFinite(Maximum);

			if (Minimum > Maximum)
			{
				throw new OptionsValidationException(this,
					$"{nameof(Minimum)} ({Minimum}) cannot be greater than {nameof(Maximum)} ({Maximum}).");
			}
		}

		/// <summary>
		/// Checks equality with another options instance:
		/// - returns <see langword="false"/> if null or type differs,
		/// - returns <see langword="true"/> if same reference,
		/// - otherwise calls <see cref="Equals(TDelayOptions?)"/>.
		/// </summary>
		public sealed override bool Equals([NotNullWhen(true)] object? obj)
		{
			if (obj is null || obj.GetType() != typeof(TDelayOptions))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			return Equals((TDelayOptions?)obj);
		}

		/// <summary>
		/// Determines equality with another instance of the same options type.
		/// </summary>
		/// <param name="other">The other instance to compare.</param>
		/// <returns><see langword="true"/> if all base fields are equal; otherwise <see langword="false"/>.</returns>
		public virtual bool Equals(TDelayOptions? other)
		{
			return other != null &&
				other.Minimum == Minimum &&
				other.Maximum == Maximum &&
				other.TimeUnit == TimeUnit;
		}

		/// <summary>
		/// Returns a hash code for the current options instance.
		/// Only includes fields relevant to base equality.
		/// Derived classes should override if they add new fields affecting equality.
		/// </summary>
		public override int GetHashCode() =>
			HashCode.Combine(Minimum, Maximum, TimeUnit);

		/// <summary>
		/// Ensures that a numeric value is finite.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="name">Optional parameter name, automatically provided by the compiler.</param>
		/// <exception cref="OptionsValidationException">Thrown if value is NaN or infinite.</exception>
		protected void EnsureFinite(double value, [CallerArgumentExpression(nameof(value))] string? name = null)
		{
			if (!double.IsFinite(value))
			{
				throw new OptionsValidationException(this,
					$"{name} ({value}) must be a finite numeric value.");
			}
		}

		/// <summary>
		/// Ensures that the configured range is wide enough to produce meaningful random values.
		/// </summary>
		/// <remarks>
		/// The range length is defined as Maximum - Minimum.  
		/// If the length is too small (≤ double.Epsilon), random generation would yield nearly constant values.
		/// </remarks>
		/// <exception cref="OptionsValidationException">
		/// Thrown if the configured range length is too small to generate meaningful randomness.
		/// </exception>
		protected void EnsureValidRange()
		{
			double length = Maximum - Minimum;

			if (length <= double.Epsilon)
			{
				throw new OptionsValidationException(this,
					"Configured range is too short to generate meaningful randomness. " +
					"The 'length' of the interval (Maximum - Minimum) must be greater than zero. " +
					"Please ensure Maximum is sufficiently larger than Minimum.");
			}
		}
	}
}
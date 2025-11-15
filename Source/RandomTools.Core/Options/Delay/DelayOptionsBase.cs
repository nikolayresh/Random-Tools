using RandomTools.Core.Exceptions;

namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Base class for configuring delay ranges expressed in a numeric interval
	/// and a <see cref="TimeUnit"/>. This class supports a fluent configuration
	/// pattern through its generic type parameter, allowing derived types to
	/// return their own type from configuration methods.
	/// <br/><br/>
	/// A delay range is defined by a closed interval: <c>[Minimum, Maximum]</c>.
	/// The interval may represent:
	/// <list type="bullet">
	/// <item><description>a normal range (e.g., 100–500 ms)</description></item>
	/// <item><description>a fixed deterministic delay (e.g., 250–250 ms)</description></item>
	/// <item><description>a zero-delay case (0–0), allowed for testing scenarios</description></item>
	/// </list>
	/// The only invalid numeric cases are:
	/// <list type="bullet">
	/// <item><description><c>Minimum &lt; 0</c></description></item>
	/// <item><description><c>Maximum &lt; 0</c></description></item>
	/// <item><description><c>Minimum &gt; Maximum</c></description></item>
	/// </list>
	/// </summary>
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
		/// Must be non-negative. A value of zero is allowed, including in the
		/// special deterministic case <c>[0,0]</c>.
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
		/// <param name="value">The minimum delay value. Must be ≥ 0.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMinimum(double value)
		{
			Minimum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the maximum delay value.
		/// </summary>
		/// <param name="value">The maximum delay value. Must be ≥ 0.</param>
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
		/// Validates the configured delay interval to ensure it is logically consistent.
		/// The validation rules are intentionally minimal:
		/// <list type="bullet">
		/// <item><description><c>Minimum</c> must be non-negative.</description></item>
		/// <item><description><c>Maximum</c> must be non-negative.</description></item>
		/// <item><description><c>Minimum</c> must not exceed <c>Maximum</c>.</description></item>
		/// </list>
		/// This allows both normal ranges (e.g. 100–500 ms) and degenerate deterministic
		/// ranges such as <c>[250,250]</c> or <c>[0,0]</c>.</summary>
		/// <exception cref="OptionsValidationException">
		/// Thrown if any of the validation rules are violated.
		/// </exception>
		public virtual void Validate()
		{
			if (Minimum > Maximum)
			{
				throw new OptionsValidationException(this,
					$"{nameof(Minimum)} ({Minimum}) cannot be greater than {nameof(Maximum)} ({Maximum}).");
			}
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			if (obj is not TDelayOptions other)
				return false;

			return Minimum == other.Minimum &&
				   Maximum == other.Maximum &&
				   TimeUnit == other.TimeUnit;
		}

		/// <inheritdoc />
		public override int GetHashCode() =>
			HashCode.Combine(Minimum, Maximum, TimeUnit);
	}
}
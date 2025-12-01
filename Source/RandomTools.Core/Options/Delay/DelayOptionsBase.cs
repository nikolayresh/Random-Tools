using RandomTools.Core.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace RandomTools.Core.Options.Delay
{
	/// <summary>
	/// Base class for configuring numeric delay ranges with a specific <see cref="TimeUnit"/>.
	/// Supports fluent configuration through the generic type parameter.
	/// </summary>
	/// <typeparam name="TDelayOptions">The concrete type used to enable fluent API calls.</typeparam>
	public abstract class DelayOptionsBase<TDelayOptions> : IOptionsBase, IEquatable<TDelayOptions>
		where TDelayOptions : DelayOptionsBase<TDelayOptions>
	{
		/// <summary>
		/// Minimum value of the delay range.
		/// Can be negative, zero, or positive.
		/// </summary>
		internal double Minimum;

		/// <summary>
		/// Maximum value of the delay range.
		/// Can be negative, zero, or positive.
		/// </summary>
		internal double Maximum;

		/// <summary>
		/// Time unit applied to <see cref="Minimum"/> and <see cref="Maximum"/>.
		/// </summary>
		internal TimeUnit TimeUnit;

		/// <summary>
		/// Sets the minimum delay value.
		/// </summary>
		/// <param name="value">Minimum value, which can be negative, zero, or positive.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMinimum(double value)
		{
			Minimum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the maximum delay value.
		/// </summary>
		/// <param name="value">Maximum value, which can be negative, zero, or positive.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithMaximum(double value)
		{
			Maximum = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Sets the time unit for interpreting the delay values.
		/// </summary>
		/// <param name="value">Time unit.</param>
		/// <returns>The current instance for fluent configuration.</returns>
		public TDelayOptions WithTimeUnit(TimeUnit value)
		{
			TimeUnit = value;
			return (TDelayOptions)this;
		}

		/// <summary>
		/// Validates the configured delay range for consistency and usability.
		/// </summary>
		/// <exception cref="OptionsValidationException">
		/// Thrown if values are not finite or if <see cref="Minimum"/> exceeds <see cref="Maximum"/>,
		/// or if the interval is too narrow to be meaningful.
		/// </exception>
		public virtual void Validate()
		{
			EnsureFinite(Minimum);
			EnsureFinite(Maximum);

			if (Minimum > Maximum)
			{
				throw new OptionsValidationException(this,
					$"Invalid delay range: Minimum ({Minimum}) cannot be greater than Maximum ({Maximum}). " +
					"The interval results in a negative length.");
			}

			double range = Maximum - Minimum;
			if (range <= double.Epsilon)
			{
				throw new OptionsValidationException(this,
					$"Invalid delay range - the interval is too narrow. Maximum ({Maximum}) must be sufficiently " +
					$"greater than Minimum ({Minimum}) to allow meaningful random delays. " +
					$"The calculated range ({range}) is too close to zero.");
			}
		}

		/// <summary>
		/// Determines equality with another object.
		/// Returns false if null or type differs, true if same reference, otherwise calls <see cref="Equals(TDelayOptions?)"/>.
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
		/// Determines equality with another instance of the same type.
		/// </summary>
		/// <param name="other">Instance to compare.</param>
		/// <returns>True if all base fields are equal; otherwise false.</returns>
		public virtual bool Equals(TDelayOptions? other) =>
			other != null &&
			other.Minimum == Minimum &&
			other.Maximum == Maximum &&
			other.TimeUnit == TimeUnit;

		/// <summary>
		/// Returns a hash code based on the base fields.
		/// Derived classes should override if they add new fields affecting equality.
		/// </summary>
		public override int GetHashCode() =>
			HashCode.Combine(Minimum, Maximum, (int)TimeUnit);

		/// <summary>
		/// Ensures a numeric value is finite.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="name">Optional parameter name, auto-filled by compiler.</param>
		/// <exception cref="OptionsValidationException">Thrown if the value is NaN or infinite.</exception>
		protected void EnsureFinite(double value, [CallerArgumentExpression(nameof(value))] string? name = null)
		{
			if (!double.IsFinite(value))
			{
				throw new OptionsValidationException(this,
					$"{name} ({value}) must be a finite numeric value.");
			}
		}
	}
}
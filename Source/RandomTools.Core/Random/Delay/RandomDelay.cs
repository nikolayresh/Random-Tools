using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays represented as <see cref="TimeSpan"/>.
	/// <para>
	/// This class provides both synchronous and asynchronous methods for introducing
	/// random delays, which can be used for:
	/// - Adding jitter to retry policies
	/// - Throttling operations
	/// - Simulating network or processing latency in tests
	/// </para>
	/// <para>
	/// The actual delay duration is determined by the <see cref="Next"/> method
	/// inherited from <see cref="RandomBase{TResult, TOptions}"/>. Derived classes
	/// define the random distribution and range via <typeparamref name="TOptions"/>.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">
	/// Type of options controlling the random delay behavior.
	/// Must inherit from <see cref="DelayOptionsBase{TOptions}"/>.
	/// </typeparam>
	public abstract class RandomDelay<TOptions> : RandomBase<TimeSpan, TOptions>
		where TOptions : DelayOptionsBase<TOptions>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RandomDelay{TOptions}"/> with the given options.
		/// </summary>
		/// <param name="options">
		/// Configuration defining how delays are calculated, including range and distribution parameters.
		/// </param>
#pragma warning disable IDE0290 // Use primary constructor
		protected RandomDelay(TOptions options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
			// Base class stores options and provides the Next() method for generating delays.
		}

		/// <summary>
		/// Synchronously waits for a randomly generated delay.
		/// <para>
		/// Internally, this uses <see cref="Thread.Sleep"/> to block the current thread
		/// for the duration returned by <see cref="Next"/>. If the computed delay is zero
		/// or negative, the method returns immediately.
		/// </para>
		/// <para>
		/// Use this method when synchronous blocking is acceptable. For non-blocking asynchronous
		/// workflows, prefer <see cref="WaitAsync"/>.
		/// </para>
		/// </summary>
		/// <returns>The actual <see cref="TimeSpan"/> delay that was waited.</returns>
		public TimeSpan Wait()
		{
			TimeSpan delay = Next();

			if (delay > TimeSpan.Zero)
			{
				// Block the current thread for the computed delay.
				Thread.Sleep(delay);
			}

			return delay;
		}

		/// <summary>
		/// Asynchronously waits for a randomly generated delay.
		/// <para>
		/// Uses <see cref="Task.Delay(TimeSpan, CancellationToken)"/> to asynchronously
		/// wait without blocking the current thread. Supports cancellation via the
		/// <paramref name="cancellationToken"/> parameter.
		/// </para>
		/// <para>
		/// This method is ideal for asynchronous workflows, e.g., introducing
		/// jitter in HTTP retries or simulating asynchronous processing delays.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">
		/// Optional <see cref="CancellationToken"/> to cancel the delay early.
		/// Default is <see cref="CancellationToken.None"/>.
		/// </param>
		/// <returns>
		/// A <see cref="Task{TimeSpan}"/> that completes after the computed delay,
		/// or is canceled if the <paramref name="cancellationToken"/> is triggered.
		/// </returns>
		public async Task<TimeSpan> WaitAsync(CancellationToken cancellationToken = default)
		{
			TimeSpan delay = Next();

			if (delay > TimeSpan.Zero)
			{
				// Asynchronously wait for the computed delay, supporting cancellation.
				await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
			}

			return delay;
		}

		/// <summary>
		/// Scales a fraction in the range [0,1] to the configured [Minimum, Maximum] range.
		/// </summary>
		/// <param name="fraction">
		/// A value between 0 and 1, typically produced by the underlying random distribution.
		/// </param>
		/// <returns>
		/// The value scaled linearly to the range [Minimum, Maximum].
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="fraction"/> is less than 0 or greater than 1.
		/// </exception>
		protected double ScaleToRange(double fraction)
		{
			// Validate that the fraction is within the normalized [0,1] range
			ArgumentOutOfRangeException.ThrowIfNegative(fraction);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(fraction, 1.0);

			// Linearly scale fraction to the configured range
			double range = Options.Maximum - Options.Minimum;
			return Math.FusedMultiplyAdd(range, fraction, Options.Minimum);
		}
	}
}

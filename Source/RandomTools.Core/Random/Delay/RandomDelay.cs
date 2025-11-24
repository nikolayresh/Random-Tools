using RandomTools.Core.Options.Delay;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays as <see cref="TimeSpan"/>.
	/// <para>
	/// Provides both synchronous and asynchronous waiting methods. Typical use cases:
	/// - Adding jitter to retry policies
	/// - Throttling operations
	/// - Simulating network or processing latency in tests
	/// </para>
	/// <para>
	/// Derived classes define the random distribution via <typeparamref name="TOptions"/>.
	/// Delay values are produced by the <see cref="Next"/> method inherited
	/// from <see cref="RandomBase{TResult, TOptions}"/>.
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
		/// Threshold (in milliseconds) before switching from <see cref="Thread.Sleep"/>
		/// to a busy-wait (<see cref="SpinWait"/>) for higher precision.
		/// Delays longer than this threshold are mostly slept,
		/// while the last few milliseconds are spin-waited to reduce timing inaccuracies.
		/// </summary>
		private const int SpinWaitThresholdMs = 20;

		/// <summary>
		/// Initializes a new instance of <see cref="RandomDelay{TOptions}"/> with the specified options.
		/// </summary>
		/// <param name="options">
		/// Configuration defining range and distribution parameters for the delay.
		/// </param>
#pragma warning disable IDE0290 // Use primary constructor
		protected RandomDelay(TOptions options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
			// Options are stored by the base class. The Next() method generates the delay values.
		}

		/// <summary>
		/// Synchronously waits for a randomly generated delay.
		/// <para>
		/// This method uses a hybrid approach:
		/// - For long delays (> <see cref="SpinWaitThresholdMs"/>), the thread is blocked with <see cref="Thread.Sleep"/>
		///   to conserve CPU.
		/// - For the remaining fraction of the delay, <see cref="SpinWait"/> is used to improve accuracy.
		/// </para>
		/// <para>
		/// Use this method when synchronous blocking is acceptable. Avoid calling on thread pool
		/// threads in high-concurrency scenarios, as it blocks the thread.
		/// </para>
		/// </summary>
		/// <returns>The actual <see cref="TimeSpan"/> that was waited.</returns>
		public TimeSpan Wait()
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			long startTicks = Stopwatch.GetTimestamp();
			long targetTicks = startTicks + (long)(delay.TotalSeconds * Stopwatch.Frequency);

			// Sleep most of the delay to reduce CPU usage
			if (delay.TotalMilliseconds > SpinWaitThresholdMs)
			{
				TimeSpan sleepTime = delay - TimeSpan.FromMilliseconds(SpinWaitThresholdMs);
				Thread.Sleep(sleepTime);
			}

			// Busy-wait the remaining time
			var spinWait = new SpinWait();
			while (Stopwatch.GetTimestamp() < targetTicks)
			{
				spinWait.SpinOnce();
			}

			return delay;
		}

		/// <summary>
		/// Asynchronously waits for a randomly generated delay.
		/// <para>
		/// Uses <see cref="Task.Delay(TimeSpan, CancellationToken)"/> to avoid blocking the thread.
		/// Supports cancellation via the <paramref name="cancellationToken"/>.
		/// </para>
		/// <para>
		/// Preferred for asynchronous workflows such as HTTP retries, background tasks,
		/// or simulating latency without blocking threads.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">
		/// Token used to cancel the delay early. Defaults to <see cref="CancellationToken.None"/>.
		/// </param>
		/// <returns>A <see cref="Task{TimeSpan}"/> that completes after the delay or is canceled.</returns>
		public async Task<TimeSpan> WaitAsync(CancellationToken cancellationToken = default)
		{
			TimeSpan delay = Next();

			// Cancel immediately if requested
			cancellationToken.ThrowIfCancellationRequested();

			if (delay > TimeSpan.Zero)
			{
				await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
			}

			return delay;
		}

		/// <summary>
		/// Scales a normalized fraction [0,1] from the underlying distribution
		/// to the configured [Minimum, Maximum] range.
		/// </summary>
		/// <param name="fraction">
		/// A value between 0 and 1 produced by the random distribution.
		/// </param>
		/// <returns>A linearly scaled value in the configured range [Minimum, Maximum].</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="fraction"/> is outside the [0,1] range.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected double ScaleToRange(double fraction)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(fraction);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(fraction, 1.0);

			return Math.FusedMultiplyAdd(
				Options.Maximum - Options.Minimum,
				fraction,
				Options.Minimum);
		}
	}
}
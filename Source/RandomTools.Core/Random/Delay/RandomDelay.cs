using RandomTools.Core.Options.Delay;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays as <see cref="TimeSpan"/>.
	/// <para>
	/// Provides synchronous and asynchronous waiting methods with a hybrid approach:
	/// sleeps for long delays and spin-waits for short remaining durations for higher precision.
	/// </para>
	/// <para>
	/// Derived classes define the random distribution via <typeparamref name="TOptions"/>.
	/// The delay values are produced by the <see cref="Next"/> method from <see cref="RandomBase{TResult, TOptions}"/>.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">
	/// Options type controlling the random delay behavior, inheriting from <see cref="DelayOptionsBase{TOptions}"/>.
	/// </typeparam>
	public abstract class RandomDelay<TOptions> : RandomBase<TimeSpan, TOptions>
		where TOptions : DelayOptionsBase<TOptions>
	{
		/// <summary>
		/// Threshold before switching from <see cref="Thread.Sleep"/> to busy-wait (<see cref="SpinWait"/>)
		/// for higher timing precision.
		/// Delays above this threshold are mostly slept; the remainder is spin-waited.
		/// </summary>
		private static readonly TimeSpan SpinWaitThreshold = TimeSpan.FromMilliseconds(20);

		/// <summary>
		/// Initializes a new instance with the specified options.
		/// </summary>
		/// <param name="options">Configuration defining range and distribution parameters for the delay.</param>
#pragma warning disable IDE0290
		protected RandomDelay(TOptions options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Synchronously waits for a randomly generated delay.
		/// <para>
		/// Uses a hybrid approach: sleeps for the bulk of the delay and spin-waits for the remaining fraction
		/// to improve accuracy.
		/// </para>
		/// <para>
		/// Suitable for scenarios where blocking the current thread is acceptable.
		/// Avoid using on thread pool threads under high concurrency.
		/// </para>
		/// </summary>
		/// <returns>The actual <see cref="TimeSpan"/> that was waited.</returns>
		public TimeSpan Wait()
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			var stopWatch = Stopwatch.StartNew();

			while (true)
			{
				TimeSpan diff = delay - stopWatch.Elapsed;

				if (diff > SpinWaitThreshold)
				{
					TimeSpan sleepTime = diff - SpinWaitThreshold;

					// Prevent negative or zero sleep due to rounding
					if (sleepTime.Ticks > 0L)
						Thread.Sleep(sleepTime);

					continue;
				}

				break;
			}

			// If the delay has already elapsed, return immediately
			if (stopWatch.Elapsed >= delay)
				return delay;

			// Spin-wait for the remaining time to improve precision
			var spinWait = new SpinWait();
			while (stopWatch.Elapsed < delay)
				spinWait.SpinOnce();

			return delay;
		}

		/// <summary>
		/// Asynchronously waits for a randomly generated delay.
		/// <para>
		/// Uses <see cref="Task.Delay(TimeSpan, CancellationToken)"/> to avoid blocking the thread.
		/// Supports cancellation via the <paramref name="cancellationToken"/>.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">Token to cancel the delay early. Defaults to <see cref="CancellationToken.None"/>.</param>
		/// <returns>A <see cref="Task{TimeSpan}"/> that completes after the delay or is canceled.</returns>
		public async Task<TimeSpan> WaitAsync(CancellationToken cancellationToken = default)
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			cancellationToken.ThrowIfCancellationRequested();

			await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

			return delay;
		}

		/// <summary>
		/// Maps a normalized value in [0,1] to the configured range <see cref="Options.Minimum"/> to <see cref="Options.Maximum"/>.
		/// </summary>
		/// <param name="factor">Normalized interpolation factor between 0.0 and 1.0</param>
		/// <returns>Linearly scaled value within the configured range.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="factor"/> is outside [0,1].</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected double ScaleToRange(double factor)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(factor);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(factor, 1.0);

			return Math.FusedMultiplyAdd(
				factor, 
				Options.Maximum - Options.Minimum, 
				Options.Minimum);
		}
	}
}

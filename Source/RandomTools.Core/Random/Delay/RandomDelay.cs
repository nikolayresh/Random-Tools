using RandomTools.Core.Options.Delay;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays expressed as <see cref="TimeSpan"/>.
	/// <para>
	/// Provides both synchronous and asynchronous waiting mechanisms.
	/// Derived types define the underlying distribution via <typeparamref name="TOptions"/>.
	/// </para>
	/// <para>
	/// Synchronous waiting uses a high-precision CPU-bound spin loop with <see cref="Stopwatch"/>
	/// and a <see cref="FastSpinner"/> to balance CPU usage while maintaining accurate timing.
	/// Asynchronous waiting leverages <see cref="Task.Delay"/> for non-blocking delays.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">
	/// Configuration type controlling delay generation. Must inherit from <see cref="DelayOptionsBase{TOptions}"/>.
	/// </typeparam>
	public abstract class RandomDelay<TOptions> : RandomBase<TimeSpan, TOptions>
		where TOptions : DelayOptionsBase<TOptions>
	{
		/// <summary>
		/// Initializes a new <see cref="RandomDelay{TOptions}"/> instance with the specified options.
		/// </summary>
		/// <param name="options">The configuration controlling delay generation.</param>
#pragma warning disable IDE0290
		protected RandomDelay(TOptions options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Performs a synchronous wait for a randomly generated delay.
		/// <para>
		/// The delay is obtained via <see cref="Next"/>, then a high-precision spin loop is used
		/// to wait for the exact duration. This approach dedicates a CPU core for the duration
		/// of the wait but provides the most precise timing possible.
		/// </para>
		/// <para>
		/// A <see cref="FastSpinner"/> is used inside the loop to prevent excessive CPU spinning
		/// while still maintaining microsecond-level accuracy.
		/// </para>
		/// </summary>
		/// <returns>The actual <see cref="TimeSpan"/> that was waited.</returns>
		public TimeSpan Wait()
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			// Convert the delay to Stopwatch ticks for high-precision comparison
			double seconds = delay.Ticks / (double)TimeSpan.TicksPerSecond;
			long targetTicks = (long)Math.Round(seconds * Stopwatch.Frequency) + Stopwatch.GetTimestamp();

			// Use a spinner to efficiently spin until the target time is reached
			FastSpinner spinner = new();

			while (Stopwatch.GetTimestamp() < targetTicks)
			{
				spinner.SpinOnce();
			}

			return delay;
		}

		/// <summary>
		/// Performs an asynchronous wait for a randomly generated delay.
		/// <para>
		/// This method is non-blocking and suitable for asynchronous or high-concurrency scenarios.
		/// It uses <see cref="Task.Delay"/> to release the thread while waiting.
		/// </para>
		/// <para>
		/// Cancellation is supported via the optional <paramref name="cancellationToken"/>.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">Token to cancel the delay before it completes.</param>
		/// <returns>
		/// A <see cref="Task{TimeSpan}"/> that completes after the generated delay
		/// or immediately if the operation is cancelled.
		/// </returns>
		public async Task<TimeSpan> WaitAsync(CancellationToken cancellationToken = default)
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			// Check for cancellation before starting the delay
			cancellationToken.ThrowIfCancellationRequested();

			// Await the asynchronous delay without blocking the calling thread
			await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

			return delay;
		}

		/// <summary>
		/// Maps a normalized value in the [0, 1] interval to the configured delay range
		/// defined by <see cref="Options.Minimum"/> and <see cref="Options.Maximum"/>.
		/// <para>
		/// Uses <see cref="Math.FusedMultiplyAdd(double, double, double)"/> to minimize
		/// rounding errors during the linear interpolation.
		/// </para>
		/// </summary>
		/// <param name="factor">Normalized factor, must be between 0.0 and 1.0 inclusive.</param>
		/// <returns>A value linearly interpolated within the configured delay range.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="factor"/> is outside the [0, 1] interval.
		/// </exception>
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

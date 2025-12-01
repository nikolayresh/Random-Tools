using RandomTools.Core.Options.Delay;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays expressed as <see cref="TimeSpan"/>.
	/// <para>
	/// Provides synchronous and asynchronous waiting mechanisms. 
	/// The synchronous version implements a high-precision CPU-bound wait using
	/// <see cref="SpinWait"/> and <see cref="Stopwatch"/> for accurate timing.
	/// </para>
	/// <para>
	/// Derived types define the underlying distribution through the supplied
	/// <typeparamref name="TOptions"/> and obtain delay values via
	/// <see cref="RandomBase{TResult, TOptions}.Next"/>.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">
	/// Configuration type controlling delay generation. Must inherit from
	/// <see cref="DelayOptionsBase{TOptions}"/>.
	/// </typeparam>
	public abstract class RandomDelay<TOptions> : RandomBase<TimeSpan, TOptions>
		where TOptions : DelayOptionsBase<TOptions>
	{
		/// <summary>
		/// Initializes a new <see cref="RandomDelay{TOptions}"/> instance 
		/// with the provided options.
		/// </summary>
		/// <param name="options">The delay configuration.</param>
#pragma warning disable IDE0290
		protected RandomDelay(TOptions options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Performs a synchronous wait for a randomly generated delay.
		/// <para>
		/// This method uses a tight spin-loop backed by <see cref="SpinWait"/>
		/// and timestamp comparisons via <see cref="Stopwatch"/>. It never yields 
		/// or sleeps, ensuring the highest possible timing precision at the cost
		/// of dedicating a full CPU core for the duration of the wait.
		/// </para>
		/// <para>
		/// Recommended only when sub-millisecond accuracy is required and blocking
		/// the calling thread is acceptable.
		/// </para>
		/// </summary>
		/// <returns>The generated delay that was waited.</returns>
		public TimeSpan Wait()
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			// High-precision conversion
			double delaySeconds = delay.Ticks / (double)TimeSpan.TicksPerSecond;
			long endTicks = ((long)(delaySeconds * Stopwatch.Frequency)) + Stopwatch.GetTimestamp();

			SpinWait spinner = new();

			while (true)
			{
				long now = Stopwatch.GetTimestamp();
				if (now >= endTicks)
					break;

				// Prevent SpinWait from entering yield/backoff phases.
				if (spinner.NextSpinWillYield)
					spinner.Reset();

				spinner.SpinOnce();
			}

			return delay;
		}

		/// <summary>
		/// Performs an asynchronous wait for a randomly generated delay.
		/// <para>
		/// Uses <see cref="Task.Delay(TimeSpan, CancellationToken)"/> and therefore
		/// does not block the thread or consume CPU resources. This is the preferred 
		/// method in asynchronous or high-concurrency scenarios.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">Optional token used to cancel the operation.</param>
		/// <returns>
		/// A task that completes after the generated delay or when cancellation is requested.
		/// </returns>
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
		/// Maps a normalized value in the [0, 1] interval to the configured delay range
		/// <see cref="Options.Minimum"/>–<see cref="Options.Maximum"/>.
		/// <para>
		/// Uses <see cref="Math.FusedMultiplyAdd(double, double, double)"/> to perform
		/// the interpolation with minimal rounding error.
		/// </para>
		/// </summary>
		/// <param name="factor">A normalized factor between 0.0 and 1.0.</param>
		/// <returns>A value linearly interpolated within the configured range.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="factor"/> is outside the [0, 1] range.
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
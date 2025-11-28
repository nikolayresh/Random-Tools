using RandomTools.Core.Options.Delay;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays represented as <see cref="TimeSpan"/>.
	/// <para>
	/// Provides both synchronous and asynchronous wait mechanisms. 
	/// The synchronous method implements a high-precision spin-wait using 
	/// <see cref="Stopwatch"/> for time measurement and <see cref="SpinWait"/> 
	/// for CPU-side delay control.
	/// </para>
	/// <para>
	/// Derived classes define the underlying random distribution by supplying 
	/// <typeparamref name="TOptions"/>, and obtain delay values through
	/// the inherited <see cref="RandomBase{TResult, TOptions}.Next"/> method.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">
	/// Options controlling the delay generation logic.
	/// Must inherit from <see cref="DelayOptionsBase{TOptions}"/>.
	/// </typeparam>
	public abstract class RandomDelay<TOptions> : RandomBase<TimeSpan, TOptions>
		where TOptions : DelayOptionsBase<TOptions>
	{
		/// <summary>
		/// Initializes a new <see cref="RandomDelay{TOptions}"/> instance with the specified options.
		/// </summary>
		/// <param name="options">Configuration that defines the delay range and distribution.</param>
#pragma warning disable IDE0290
		protected RandomDelay(TOptions options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Performs a synchronous wait for a randomly generated duration.
		/// <para>
		/// Uses a strictly CPU-bound spin-loop driven by <see cref="SpinWait"/>.
		/// This method never calls <see cref="Thread.Sleep(int)"/> and never yields 
		/// the thread voluntarily, providing the highest possible timing precision.
		/// </para>
		/// <para>
		/// Because spin-waiting consumes an entire CPU core for the duration 
		/// of the wait, this method should only be used when blocking the calling 
		/// thread is acceptable and precise sub-millisecond timing matters.
		/// </para>
		/// </summary>
		/// <returns>The generated <see cref="TimeSpan"/> that was waited.</returns>
		public TimeSpan Wait()
		{
			TimeSpan delay = Next();
			if (delay <= TimeSpan.Zero)
				return delay;

			// Convert requested delay to Stopwatch ticks.
			long delayTicks = delay.Ticks * Stopwatch.Frequency / TimeSpan.TicksPerSecond;
			long endTicks = delayTicks + Stopwatch.GetTimestamp();

			SpinWait spinner = new();

			while (true)
			{
				// Get remaining ticks
				long remainingTicks = endTicks - Stopwatch.GetTimestamp();

				// Completed the wait
				if (remainingTicks <= 0L)
					break;

				// Execute one adaptive spin iteration.
				spinner.SpinOnce();
			}

			return delay;
		}

		/// <summary>
		/// Performs an asynchronous wait for a randomly generated duration.
		/// <para>
		/// Uses <see cref="Task.Delay(TimeSpan, CancellationToken)"/> to avoid blocking
		/// the calling thread. This is the preferred option for asynchronous or 
		/// high-concurrency environments, as it introduces no CPU load.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">Optional token that can cancel the wait.</param>
		/// <returns>
		/// A task that completes after the generated delay or when the operation is canceled.
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
		/// Maps a normalized value in the [0, 1] range to the configured delay range
		/// <see cref="Options.Minimum"/>–<see cref="Options.Maximum"/>.
		/// <para>
		/// Uses <see cref="Math.FusedMultiplyAdd(double, double, double)"/> to maximize
		/// numerical precision during interpolation.
		/// </para>
		/// </summary>
		/// <param name="factor">A normalized interpolation factor between 0.0 and 1.0.</param>
		/// <returns>A linearly scaled value within the configured delay range.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="factor"/> lies outside the [0, 1] interval.
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
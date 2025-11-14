using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Base class for generating random delays represented as <see cref="TimeSpan"/> values.
	/// <para>
	/// This class provides a foundation for creating asynchronous delays whose duration is determined
	/// by a random distribution specified through <typeparamref name="TOptions"/>.
	/// </para>
	/// <para>
	/// Common use cases include introducing jitter in retry policies, throttling operations,
	/// or simulating asynchronous delays in testing and simulations.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">
	/// The type of options that control the behavior of the random delay.
	/// Must inherit from <see cref="DelayOptionsBase{TOptions}"/>.
	/// </typeparam>
	public abstract class RandomDelay<TOptions> : RandomBase<TimeSpan, TOptions> where TOptions : DelayOptionsBase<TOptions>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="RandomDelay{TOptions}"/> with the specified delay options.
		/// </summary>
		/// <param name="options">
		/// The configuration options that define how the random delay is calculated
		/// (e.g., minimum and maximum delay, distribution type, etc.).
		/// </param>
		protected RandomDelay(TOptions options) : base(options)
		{
			// The base class RandomBase handles storing options and providing the Next() method.
		}

		/// <summary>
		/// Asynchronously waits for a randomly computed delay.
		/// <para>
		/// This method wraps <see cref="Task.Delay(TimeSpan, CancellationToken)"/> to provide
		/// an easy way to await a random delay. The actual delay duration is determined
		/// by the <see cref="Next"/> method from the base class.
		/// </para>
		/// <para>
		/// Supports optional cancellation via the <paramref name="cancellationToken"/>.
		/// </para>
		/// </summary>
		/// <param name="cancellationToken">
		/// A <see cref="CancellationToken"/> that can be used to cancel the wait early.
		/// Default is <see cref="CancellationToken.None"/>.
		/// </param>
		/// <returns>
		/// A <see cref="Task"/> that completes after the computed random delay, or is canceled if
		/// the <paramref name="cancellationToken"/> is triggered.
		/// </returns>
		public async Task WaitAsync(CancellationToken cancellationToken = default)
		{
			TimeSpan next = Next();
			if (next == TimeSpan.Zero)
				return;

			// Await Task.Delay for the duration returned by Next(), supporting cancellation
			await Task.Delay(next, cancellationToken).ConfigureAwait(false);
		}
	}
}

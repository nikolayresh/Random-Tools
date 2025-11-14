using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates a delay using a uniform distribution within the specified range.
	/// </summary>
	public sealed class UniformDelay : RandomDelay<DelayOptions.Uniform>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="UniformDelay"/> with the specified options.
		/// </summary>
		/// <param name="options">The configuration options for the delay.</param>
		public UniformDelay(DelayOptions.Uniform options) : base(options) { }

		/// <summary>
		/// Returns the next random delay as a <see cref="TimeSpan"/>.
		/// The value is selected uniformly from the inclusive range [Minimum, Maximum].
		/// </summary>
		public override TimeSpan Next()
		{
			// Pick a random value in [Minimum, Maximum] inclusive
			double delayValue = CoreTools.NextDouble(Options.Minimum, Options.Maximum);

			// Convert to TimeSpan using the configured unit
			return CoreTools.ToTimeSpan(delayValue, Options.TimeUnit);
		}
	}
}

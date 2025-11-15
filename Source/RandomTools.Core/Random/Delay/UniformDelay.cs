using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Produces delays based on a uniform random distribution across a configured range.
	/// </summary>
	public sealed class UniformDelay : RandomDelay<DelayOptions.Uniform>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="UniformDelay"/> using the provided options.
		/// </summary>
		/// <param name="options">The delay configuration, including range and time unit.</param>
#pragma warning disable IDE0290 // Use primary constructor
		public UniformDelay(DelayOptions.Uniform options) : base(options) { }
#pragma warning restore IDE0290 // Use primary constructor

		/// <summary>
		/// Generates the next delay value as a <see cref="TimeSpan"/>.
		/// The delay is drawn uniformly from the interval [Minimum, Maximum), meaning
		/// the minimum value is inclusive and the maximum value is excluded.
		/// Degenerate intervals such as [x, x] result in a deterministic delay.
		/// </summary>
		public override TimeSpan Next()
		{
			// Select a random double within the configured range.
			double value = CoreTools.NextDouble(Options.Minimum, Options.Maximum);

			// Convert the numeric value to a TimeSpan using the configured unit.
			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}

using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates delays following an arcsine distribution within a specified range.
	/// <para>
	/// The arcsine distribution has higher probability density near the endpoints of the interval.
	/// This can be useful when you want events to be more likely to occur near the minimum or maximum delay.
	/// </para>
	/// </summary>
	public sealed class ArcsineDelay : RandomDelay<DelayOptions.Arcsine>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ArcsineDelay"/> class
		/// using the provided configuration options.
		/// </summary>
		/// <param name="options">Configuration options defining the minimum, maximum, and time unit of the delay.</param>
#pragma warning disable IDE0290
		public ArcsineDelay(DelayOptions.Arcsine options) : base(options)
#pragma warning restore IDE0290
		{
		}

		/// <summary>
		/// Generates the next delay value according to the arcsine distribution.
		/// <para>
		/// The formula used is:
		/// <code>
		/// value = min + (max - min) * sin^2(π * u / 2)
		/// </code>
		/// where <c>u</c> is a uniform random number in [0, 1).
		/// This ensures that values near <c>min</c> and <c>max</c> are more likely.
		/// </para>
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/> representing the generated delay.</returns>
		public override TimeSpan Next()
		{
			// Generate a uniform random value in [0,1)
			double u = CoreTools.NextDouble();

			// Map the uniform value to an arcsine-distributed value
			double angle = Math.PI * u * 0.5;
			double sinSq = Math.Sin(angle) * Math.Sin(angle);

			// Scale the result to the configured range
			double scaled = ScaleToRange(sinSq);

			// Convert the numeric value to a TimeSpan using the specified time unit
			return CoreTools.ToTimeSpan(scaled, Options.TimeUnit);
		}
	}
}
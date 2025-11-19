using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates a random time delay following a clamped Exponential distribution.
	/// This distribution models the time between events in a Poisson process.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the <see cref="ExponentialDelay"/> class.
	/// </remarks>
	/// <param name="options">The options defining the rate and bounds of the exponential distribution.</param>
	public class ExponentialDelay(DelayOptions.Exponential options) : RandomDelay<DelayOptions.Exponential>(options)
	{
		/// <summary>
		/// Generates the next random delay using the Inverse Transform Method for the Exponential distribution, 
		/// then clamps the result between Minimum and Maximum bounds.
		/// </summary>
		/// <returns>A TimeSpan representing the calculated delay.</returns>
		public override TimeSpan Next()
		{
			double value;

			do
			{
				// avoid Math.Log(0)
				double u = 1.0 - CoreTools.NextDouble();

				// Apply the Inverse Transform formula: X = -ln(U) / lambda
				value = -Math.Log(u) / Options.Rate;
			} while (value < Options.Minimum || value > Options.Maximum);

			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}
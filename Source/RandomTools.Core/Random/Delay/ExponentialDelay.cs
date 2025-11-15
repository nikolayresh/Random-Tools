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
			// Retrieve the rate parameter (lambda).
			double lambda = Options.GetEffectiveLambda();

			double value;

			do
			{
				// Generate a uniform random variable U in the interval (0, 1].
				// This form (1.0 - R) avoids the possibility of U=0, which would lead to Math.Log(0) = -Infinity.
				double u = 1.0 - CoreTools.NextDouble();

				// Apply the Inverse Transform formula: X = -ln(U) / lambda.
				value = -Math.Log(u) / lambda;
			} while (value < Options.Minimum || value > Options.Maximum);

			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}
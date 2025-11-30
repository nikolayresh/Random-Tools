using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates random delays following a Beta distribution within the configured interval.
	/// <para>
	/// The Beta distribution is parameterized by <c>AlphaValue</c> and <c>BetaValue</c> from the options.
	/// The generated fraction is scaled to the interval defined by <c>Minimum</c> and <c>Maximum</c>.
	/// </para>
	/// </summary>
	public sealed class BetaDelay : RandomDelay<DelayOptions.Beta>
	{
#pragma warning disable IDE0290
		public BetaDelay(DelayOptions.Beta options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Generates the next delay as a <see cref="TimeSpan"/> sampled from a Beta distribution.
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/> representing the next randomized delay.</returns>
		public override TimeSpan Next()
		{
			// Sample two independent Gamma-distributed values for the Beta fraction
			double gammaAlpha = GammaTools.NextGamma(Options.AlphaValue, 1.0);
			double gammaBeta = GammaTools.NextGamma(Options.BetaValue, 1.0);

			// Compute Beta fraction in [0,1] using the standard relationship:
			// Beta(α, β) = Gamma(α,1) / (Gamma(α,1) + Gamma(β,1))
			double betaValue = gammaAlpha / (gammaAlpha + gammaBeta);

			// Scale the Beta fraction to the user-defined interval [Minimum, Maximum]
			double scaled = ScaleToRange(betaValue);

			// Convert the scaled value to a TimeSpan using the configured time unit
			return CoreTools.ToTimeSpan(scaled, Options.TimeUnit);
		}
	}
}
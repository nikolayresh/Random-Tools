using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates random delays based on a <b>Polynomial / Power distribution</b>.
	/// <para>
	/// The distribution generates values in [Minimum, Maximum] with a density
	/// proportional to (t - Minimum)^Power or (Maximum - t)^Power if <see cref="DelayOptions.Polynomial.Reverse"/> is true.
	/// </para>
	/// <para>
	/// A Power of 0 produces a uniform distribution. Higher Power values produce
	/// increasingly skewed delays toward Maximum (or Minimum if reversed).
	/// </para>
	/// </summary>
	public sealed class PolynomialDelay : RandomDelay<DelayOptions.Polynomial>
	{
#pragma warning disable IDE0290 // Use primary constructor
		/// <summary>
		/// Initializes a new instance of <see cref="PolynomialDelay"/> with the specified options.
		/// </summary>
		/// <param name="options">Polynomial distribution configuration options.</param>
		public PolynomialDelay(DelayOptions.Polynomial options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
		}

		/// <summary>
		/// Generates the next random delay according to the configured Polynomial distribution.
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/> representing the generated delay.</returns>
		public override TimeSpan Next()
		{
			// Generate a uniform random value in [0,1)
			double u = CoreTools.NextDouble();

			// Apply inverse CDF of the Polynomial distribution to get a normalized fraction
			// fraction ∈ [0,1], representing relative position in the [Minimum, Maximum] interval
			double fraction = Math.Pow(u, 1.0 / (Options.Power + 1.0));

			if (Options.Reverse)
			{
				// more values closer to Minimum
				fraction = 1.0 - fraction;
			}

			// Scale the normalized fraction to the [Minimum, Maximum] range
			double value = ScaleToRange(fraction);

			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}

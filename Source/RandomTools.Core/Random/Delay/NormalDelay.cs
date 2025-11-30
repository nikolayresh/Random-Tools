using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates delays using a truncated normal (Gaussian) distribution.
	/// All returned values are guaranteed to lie inside [Minimum, Maximum].
	/// <para>
	/// The generator uses rejection sampling: values falling outside the allowed
	/// interval are discarded until a valid one is produced (with bounded attempts).
	/// </para>
	/// </summary>
	public sealed class NormalDelay : RandomDelay<DelayOptions.Normal>
	{
#pragma warning disable IDE0290
		public NormalDelay(DelayOptions.Normal options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Generates one normally distributed delay using rejection sampling.
		/// Internally relies on cached Box–Muller values for performance.
		/// Values outside [Minimum, Maximum] are rejected until either:
		/// - a valid one is found, or
		/// - the generator exceeds the allowed number of attempts (rare).
		/// </summary>
		public override TimeSpan Next()
		{
			double mean = Options.Mean;
			double stdDev = Options.StandardDeviation;

			// Probability that a normal sample naturally falls inside [Min, Max].
			double pRange = GaussianTools.GetRangeHitProbability(mean, stdDev, (Options.Minimum, Options.Maximum));
			int attempts = 2 * GaussianTools.GetRequiredAttempts(pRange);

			// Cached Box–Muller value (only the current operation will use it).
			double? cache = null;
			int nextTry = attempts;

			while (true)
			{
				double value = GaussianTools.NextNormal(mean, stdDev, ref cache);

				if (value < Options.Minimum || value > Options.Maximum)
				{
					// Reject and try again.
					if (nextTry-- != 0)
						continue;

					throw new NextGeneratorException(Options,
						$"Failed to generate a normal-distribution value inside the allowed range " +
						$"[{Options.Minimum}, {Options.Maximum}] after {attempts:N0} attempts."
					);
				}

				return CoreTools.ToTimeSpan(value, Options.TimeUnit);
			}
		}
	}
}
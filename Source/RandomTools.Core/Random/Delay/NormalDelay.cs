using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates delays following a normal (Gaussian) distribution.
	/// Values are clamped to the [Minimum, Maximum] range.
	/// Supports optional Mean and StandardDeviation:
	/// - Mean defaults to midpoint of the range if not specified.
	/// - StandardDeviation defaults to 1/6 of the range if not specified.
	/// </summary>
	public sealed class NormalDelay : DelayBase<DelayOptions.Normal>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="NormalDelay"/>.
		/// </summary>
		/// <param name="options">
		/// The <see cref="DelayOptions.Normal"/> configuration for this delay generator.
		/// Includes Minimum, Maximum, optional Mean, optional StandardDeviation, and TimeUnit.
		/// </param>
		/// <remarks>
		/// The constructor does not perform validation directly; it relies on the options object.
		/// The generated delays will use a truncated normal distribution within [Minimum, Maximum].
		/// If Mean is not provided, it defaults to the midpoint of the range.
		/// If StandardDeviation is not provided, it defaults to 1/6 of the range, 
		/// ensuring ~99.7% of values fall within [Minimum, Maximum].
		/// </remarks>
		public NormalDelay(DelayOptions.Normal options) : base(options) { }

		/// <summary>
		/// Generates a single Gaussian-distributed value using the Box-Muller transform.
		/// </summary>
		/// <param name="mean">Mean of the distribution.</param>
		/// <param name="stdDev">Standard deviation of the distribution.</param>
		/// <returns>A Gaussian-distributed double value.</returns>
		private static double GenerateGaussian(double mean, double stdDev)
		{
			double u1, u2;

			do
			{
				u1 = CoreTools.NextDouble();
			} while (u1 <= double.Epsilon);

			u2 = CoreTools.NextDouble();
			double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

			return mean + standardNormal * stdDev;
		}

		/// <summary>
		/// Generates the next random delay as a TimeSpan within [Minimum, Maximum].
		/// Uses truncated Gaussian sampling.
		/// </summary>
		public override TimeSpan Next()
		{
			double mean = Options.Mean ?? (Options.Minimum + Options.Maximum) / 2.0;
			double stdDev = Options.StandardDeviation ?? (Options.Maximum - Options.Minimum) / 6.0;

			double nextValue;

			// Truncated normal: re-run if out of bounds
			do
			{
				nextValue = GenerateGaussian(mean, stdDev);
			} while (nextValue < Options.Minimum || nextValue > Options.Maximum);

			return CoreTools.ToTimeSpan(nextValue, Options.TimeUnit);
		}
	}
}

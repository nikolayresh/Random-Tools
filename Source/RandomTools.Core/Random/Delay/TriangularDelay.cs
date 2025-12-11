using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates random delays using the triangular probability distribution.
	/// The distribution is defined by a minimum value, a maximum value, and a mode (peak),
	/// all interpreted according to the configured <see cref="TimeUnit"/>.
	/// </summary>
	public sealed class TriangularDelay : RandomDelay<DelayOptions.Triangular>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TriangularDelay"/> class.
		/// </summary>
		/// <param name="options">
		/// The distribution parameters, including <see cref="DelayOptions.Triangular.Minimum"/>,
		/// <see cref="DelayOptions.Triangular.Maximum"/>, <see cref="DelayOptions.Triangular.Mode"/>,
		/// and the associated <see cref="TimeUnit"/>.
		/// </param>
#pragma warning disable IDE0290
		public TriangularDelay(DelayOptions.Triangular options) : base(options) { }
#pragma warning restore IDE0290

		/// <summary>
		/// Produces the next random delay value sampled from the triangular distribution.
		/// </summary>
		/// <returns>
		/// A <see cref="TimeSpan"/> whose duration is determined by the configured distribution.
		/// </returns>
		/// <remarks>
		/// The implementation uses the inverse cumulative distribution function (inverse CDF)
		/// to map a uniformly distributed random number onto the triangular distribution.
		/// </remarks>
		public override TimeSpan Next()
		{
			double min = Options.Minimum;
			double max = Options.Maximum;
			double mode = Options.Mode;

			// Uniform random value in [0, 1)
			double u = CoreTools.NextDouble();
			double range = max - min;

			// Position of the mode as a fraction of [min, max]
			// 0.0 = mode at min, 1.0 = mode at max
			double modePos = (mode - min) / range;
			double scaled;

			if (u < modePos)
			{
				// Sample from the left segment of the triangular CDF
				scaled = min + Math.Sqrt(u * range * (mode - min));
			}
			else
			{
				// Sample from the right segment of the triangular CDF
				scaled = max - Math.Sqrt((1.0 - u) * range * (max - mode));
			}

			// Convert the sampled numeric delay into a TimeSpan
			return CoreTools.ToTimeSpan(scaled, Options.TimeUnit);
		}
	}
}
using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates delays based on a <see cref="Bates"/> distribution.
	/// <para>
	/// The Bates distribution is defined as the arithmetic mean of 
	/// <see cref="DelayOptions.Bates.Samples"/> independent uniform random samples.
	/// It produces a smooth, bell-shaped distribution strictly within the configured 
	/// <see cref="DelayOptions.Bates.Minimum"/> and <see cref="DelayOptions.Bates.Maximum"/> range.
	/// </para>
	/// <para>
	/// When <c>Samples = 1</c>, this degenerates to a uniform distribution. Increasing 
	/// <c>Samples</c> results in a smoother, more centered distribution.
	/// </para>
	/// </summary>
	public sealed class BatesDelay : RandomDelay<DelayOptions.Bates>
	{
#pragma warning disable IDE0290 // Use primary constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="BatesDelay"/> class
		/// with the specified <see cref="DelayOptions.Bates"/>.
		/// </summary>
		/// <param name="options">Configuration options for the Bates distribution.</param>
		public BatesDelay(DelayOptions.Bates options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
		}

		/// <summary>
		/// Generates the next random delay based on the configured Bates distribution.
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/> representing the generated delay.</returns>
		public override TimeSpan Next()
		{
			// Number of uniform samples to average
			int N = Options.Samples;
			double mean = 0.0;

			for (int i = 0; i < N; i++)
			{
				double next = CoreTools.NextDouble();
				mean += (next - mean) / (i + 1);
			}

			// Map mean from [0,1] to the configured [Minimum, Maximum] range
			double value = ScaleToRange(mean);

			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}

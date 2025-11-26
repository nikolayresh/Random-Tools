using RandomTools.Core.Options.Delay;
using System;

namespace RandomTools.Core.Random.Delay
{
	/// <summary>
	/// Generates a random delay based on the Triangular distribution.
	/// </summary>
	public sealed class TriangularDelay : RandomDelay<DelayOptions.Triangular>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TriangularDelay"/> class with the specified options.
		/// </summary>
		/// <param name="options">Triangular distribution options, including Minimum, Maximum, Mode, and TimeUnit.</param>
#pragma warning disable IDE0290 // Use primary constructor
		public TriangularDelay(DelayOptions.Triangular options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
		}

		/// <summary>
		/// Generates the next random delay as a <see cref="TimeSpan"/> using the Triangular distribution.
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/> representing the random delay.</returns>
		public override TimeSpan Next()
		{
			double min = Options.Minimum;
			double max = Options.Maximum;
			double mode = Options.Mode;

			// Generate a uniform random number in [0, 1)
			double u = CoreTools.NextDouble();

			// Normalized position of the mode in the [min, max] range
			double fMode = (mode - min) / (max - min);
			double value;

			// Apply inverse CDF of triangular distribution
			if (u < fMode)
			{
				// Left side of the mode
				value = min + Math.Sqrt(u * (max - min) * (mode - min));
			}
			else
			{
				// Right side of the mode
				value = max - Math.Sqrt((1.0 - u) * (max - min) * (max - mode));
			}

			// Convert the numeric value to TimeSpan according to the specified time unit
			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}

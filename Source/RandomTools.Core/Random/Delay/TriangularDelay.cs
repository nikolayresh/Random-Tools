using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	public sealed class TriangularDelay : RandomDelay<DelayOptions.Triangular>
	{
#pragma warning disable IDE0290 // Use primary constructor
		public TriangularDelay(DelayOptions.Triangular options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
		}

		public override TimeSpan Next()
		{
			double min = Options.Minimum;
			double max = Options.Maximum;
			double mode = Options.Mode;

			double u = CoreTools.NextDouble();
			double fMode = (mode - min) / (max - min);

			double value;

			if (u < fMode)
			{
				value = min + Math.Sqrt(u * (max - min) * (mode - min));
			}
			else
			{
				value = max - Math.Sqrt((1.0 - u) * (max - min) * (max - mode));
			}

			return CoreTools.ToTimeSpan(value, Options.TimeUnit);
		}
	}
}

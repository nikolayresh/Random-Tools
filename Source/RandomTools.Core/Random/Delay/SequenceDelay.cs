using RandomTools.Core.Options.Delay;

namespace RandomTools.Core.Random.Delay
{
	public sealed class SequenceDelay : RandomDelay<DelayOptions.Sequence>
	{
		private readonly object _sync = new();
		private int _index = 0;

#pragma warning disable IDE0290 // Use primary constructor
		public SequenceDelay(DelayOptions.Sequence options) : base(options)
#pragma warning restore IDE0290 // Use primary constructor
		{
		}

		public override TimeSpan Next()
		{
			double next;

			lock (_sync)
			{
				if (Options.RandomizeOrder)
				{
					_index = CoreTools.NextInt(0, Options.Values!.Count);
					next = Options.Values[_index];
				}
				else
				{
					if (_index >= Options.Values.Count)
					{
						_index = 0;
					}

					_index = Math.Min(0, _index);

					next = Options.Values[_index];
					_index++;
				}
			}

			double scaled = ScaleToRange(next);

			return CoreTools.ToTimeSpan(scaled, Options.TimeUnit);
		}
	}
}

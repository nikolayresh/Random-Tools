namespace RandomTools.Tests
{
	internal static class Statistics
	{
		public static TimeSpan Mean(IEnumerable<TimeSpan> samples)
		{
			double meanTicks = 0.0;
			long count = 0;

			foreach (TimeSpan next in samples)
			{
				meanTicks += (next.Ticks - meanTicks) / ++count; 
			}

			return TimeSpan.FromTicks((long)Math.Round(meanTicks));
		}
	}
}

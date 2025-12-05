using FluentAssertions;
using RandomTools.Core;

namespace RandomTools.Tests
{
	[TestFixture]
	public class BatesDelayTests
	{
		private const int Iterations = 1_000_000;
		private List<TimeSpan> _delays;

		[SetUp]
		public void SetUp()
		{
			_delays ??= [];
			_delays.Clear();
		}

		[Test, Retry(3)]
		public void When_Sampling_Minutes(
			[Values(1.5, 2.0)] double min,
			[Values(3.5, 4.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double midpoint = (min + max) / 2.0;
			double stdDev = (max - min) / Math.Sqrt(12.0 * samples);
			double tolerance = 3 * stdDev / Math.Sqrt(Iterations);

			var delay = RandomTool.Delay.Bates.For(min, max, samples, TimeUnit.Minute);

			for (int i = 0; i < Iterations; i++)
			{
				_delays.Add(delay.Next());
			}

			var mean = Statistics.Mean(_delays);

			mean.TotalMinutes.Should().BeApproximately(midpoint, tolerance);
		}

		[Test, Retry(3)]
		public void When_Sampling_Seconds(
			[Values(10.0, 20.0)] double min,
			[Values(30.0, 50.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double midpoint = (min + max) / 2.0;
			double stdDev = (max - min) / Math.Sqrt(12.0 * samples);
			double tolerance = 3 * stdDev / Math.Sqrt(Iterations);

			var delay = RandomTool.Delay.Bates.For(min, max, samples, TimeUnit.Second);

			for (int i = 0; i < Iterations; i++)
			{
				_delays.Add(delay.Next());
			}

			var mean = Statistics.Mean(_delays);

			mean.TotalSeconds.Should().BeApproximately(midpoint, tolerance);
		}
	}
}

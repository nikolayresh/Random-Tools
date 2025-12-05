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

		[Test, Combinatorial]
		public void When_Sampling_MilliSeconds(
			[Values(1_000.0, 5_000.0)] double min,
			[Values(10_000.0, 15_000.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double theoryMean = Statistics.Theory.Mean.Bates(min, max);
			double theoryStdDev = Statistics.Theory.StandardDeviation.Bates(min, max, samples);
			double tolerance = 3 * Statistics.SEM(theoryStdDev, Iterations);

			var delay = RandomTool.Delay.Bates.InMilliseconds(min, max, samples);

			for (int i = 0; i < Iterations; i++)
			{
				TimeSpan next = delay.Next();
				next.TotalMilliseconds
					.Should()
					.BeInRange(min, max);

				_delays.Add(next);
			}

			TimeSpan mean = Statistics.Mean(_delays);

			mean.TotalMilliseconds
				.Should()
				.BeApproximately(theoryMean, tolerance);
		}

		[Test, Combinatorial]
		public void When_Sampling_Minutes(
			[Values(1.5, 2.0)] double min,
			[Values(3.5, 4.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double theoryMean = Statistics.Theory.Mean.Bates(min, max);
			double theoryStdDev = Statistics.Theory.StandardDeviation.Bates(min, max, samples);
			double tolerance = 3 * Statistics.SEM(theoryStdDev, Iterations);

			var delay = RandomTool.Delay.Bates.InMinutes(min, max, samples);

			for (int i = 0; i < Iterations; i++)
			{
				TimeSpan next = delay.Next();
				next.TotalMinutes
					.Should()
					.BeInRange(min, max);

				_delays.Add(next);
			}

			TimeSpan mean = Statistics.Mean(_delays);

			mean.TotalMinutes
				.Should()
				.BeApproximately(theoryMean, tolerance);
		}

		[Test, Combinatorial]
		public void When_Sampling_Seconds(
			[Values(10.0, 20.0)] double min,
			[Values(30.0, 50.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double theoryMean = Statistics.Theory.Mean.Bates(min, max);
			double theoryStdDev = Statistics.Theory.StandardDeviation.Bates(min, max, samples);
			double tolerance = 3 * Statistics.SEM(theoryStdDev, Iterations);

			var delay = RandomTool.Delay.Bates.InSeconds(min, max, samples);

			for (int i = 0; i < Iterations; i++)
			{
				TimeSpan next = delay.Next();
				next.TotalSeconds
					.Should()
					.BeInRange(min, max);

				_delays.Add(next);
			}

			TimeSpan mean = Statistics.Mean(_delays);

			mean.TotalSeconds
				.Should()
				.BeApproximately(theoryMean, tolerance);
		}
	}
}

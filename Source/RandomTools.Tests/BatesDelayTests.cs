using FluentAssertions;
using RandomTools.Core;

namespace RandomTools.Tests
{
	[TestFixture]
	public class BatesDelayTests
	{
		private const int Iterations = 100_000;
		private List<TimeSpan> _delays;

		[SetUp]
		public void SetUp()
		{
			_delays ??= [];
			_delays.Clear();
		}

		[Test, Combinatorial]
		public void When_Sampling_MilliSeconds(
			[Values( 1_000.0,  5_000.0, 10_000.0)] double min,
			[Values(15_000.0, 20_000.0, 25_000.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double expMean = (min + max) / 2.0;
			var delay = RandomTool.Delay.Bates.InMilliseconds(min, max, samples);

			for (int i = 0; i < Iterations; i++)
			{
				TimeSpan next = delay.Next();
				next.TotalMilliseconds
					.Should()
					.BeInRange(min, max);

				_delays.Add(next);
			}

			var stats = Statistics.Analyze(_delays.Select(x => x.TotalMilliseconds));
			double SEM = Statistics.StandardErrorOfMean(stats.StandardDeviation, stats.Count);

			double delta = Statistics.GetConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
			stats.Mean.Should().BeApproximately(expMean, delta);

			double nHat = Statistics.EstimateBatesOrder(stats.Variance, (min, max));
			nHat.Should().BeApproximately(samples, samples * 0.1);
		}

		[Test, Combinatorial]
		public void When_Sampling_Minutes(
			[Values(0.5, 1.5, 2.0)] double min,
			[Values(3.0, 3.5, 4.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double expMean = (min + max) / 2.0;
			var delay = RandomTool.Delay.Bates.InMinutes(min, max, samples);

			for (int i = 0; i < Iterations; i++)
			{
				TimeSpan next = delay.Next();
				next.TotalMinutes
					.Should()
					.BeInRange(min, max);

				_delays.Add(next);
			}

			var (Mean, Variance, StandardDeviation, Count) = Statistics.Analyze(_delays.Select(x => x.TotalMinutes));
			double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

			double delta = Statistics.GetConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
			Mean.Should().BeApproximately(expMean, delta);

			double nHat = Statistics.EstimateBatesOrder(Variance, (min, max));
			nHat.Should().BeApproximately(samples, samples * 0.1);
		}

		[Test, Combinatorial]
		public void When_Sampling_Seconds(
			[Values(05.0, 10.0, 15.0)] double min,
			[Values(20.0, 25.0, 30.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 100)] int samples)
		{
			double expMean = (min + max) / 2.0; 
			var delay = RandomTool.Delay.Bates.InSeconds(min, max, samples);

			for (int i = 0; i < Iterations; i++)
			{
				TimeSpan next = delay.Next();
				next.TotalSeconds
					.Should()
					.BeInRange(min, max);

				_delays.Add(next);
			}

			var (Mean, Variance, StandardDeviation, Count) = Statistics.Analyze(_delays.Select(x => x.TotalSeconds));
			double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

			double delta = Statistics.GetConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
			Mean.Should().BeApproximately(expMean, delta);

			double nHat = Statistics.EstimateBatesOrder(Variance, (min, max));
			nHat.Should().BeApproximately(samples, samples * 0.1);
		}
	}
}

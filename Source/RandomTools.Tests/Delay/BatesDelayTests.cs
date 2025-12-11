using FluentAssertions;
using RandomTools.Core;

namespace RandomTools.Tests.Delay
{
	[TestFixture]
	public class BatesDelayTests : AbstractDelayTest
	{
		[Test, Combinatorial]
		public void BatesDelays_Milliseconds_ShouldMatchRangeMeanAndOrder(
			[Values( 1_000.0,  5_000.0, 10_000.0)] double min,
			[Values(15_000.0, 20_000.0, 25_000.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 75, 100)] int samples)
		{
			double expMean = (min + max) / 2.0;
			var delay = RandomTool.Delay.Bates.InMilliseconds(min, max, samples);
			
			GenerateSamples(delay, Selector.Milliseconds, (min, max));

			WithMilliseconds(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);

				double nHat = Statistics.EstimateBatesOrder(Variance, (min, max));
				nHat.Should().BeApproximately(samples, 1.0);
			});
		}

		[Test, Combinatorial]
		public void BatesDelays_Minutes_ShouldMatchRangeMeanAndOrder(
			[Values(0.5, 1.5, 2.0)] double min,
			[Values(3.0, 3.5, 4.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 75, 100)] int samples)
		{
			double expMean = (min + max) / 2.0;
			var delay = RandomTool.Delay.Bates.InMinutes(min, max, samples);

			GenerateSamples(delay, Selector.Minutes, (min, max));

			WithMinutes(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);

				double nHat = Statistics.EstimateBatesOrder(Variance, (min, max));
				nHat.Should().BeApproximately(samples, 1.0);
			});
		}

		[Test, Combinatorial]
		public void BatesDelays_Seconds_ShouldMatchRangeMeanAndOrder(
			[Values(05.0, 10.0, 15.0)] double min,
			[Values(20.0, 25.0, 30.0)] double max,
			[Values(1, 2, 3, 4, 5, 10, 25, 50, 75, 100)] int samples)
		{
			double expMean = (min + max) / 2.0;
			var delay = RandomTool.Delay.Bates.InSeconds(min, max, samples);

			GenerateSamples(delay, Selector.Seconds, (min, max));

			WithSeconds(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);

				double nHat = Statistics.EstimateBatesOrder(Variance, (min, max));
				nHat.Should().BeApproximately(samples, 1.0);
			});
		}
	}
}
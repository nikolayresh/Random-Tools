using FluentAssertions;
using RandomTools.Core;

namespace RandomTools.Tests.Delay
{
	[TestFixture]
	public class TriangularDelayTests : AbstractDelayTest
	{
		private const double ToleranceFactor = 0.075;

		[TestCase(0.0, 5.0, 10.0)]
		[TestCase(10.0, 20.0, 30.0)]
		[TestCase(2.5, 3.0, 3.5)]
		[TestCase(1.0, 1.0, 5.0)]
		[TestCase(1.0, 5.0, 5.0)]
		[TestCase(0.0, 0.0, 0.1)]
		[TestCase(0.0, 0.1, 0.1)]
		[TestCase(5.0, 5.0001, 5.0002)]
		[TestCase(10.0, 10.0, 10.00001)]
		[TestCase(0.0, 50.0, 100.0)]
		[TestCase(0.0, 0.9, 1.0)]
		[TestCase(0.0, 0.99, 1.0)]
		[TestCase(0.0, 0.1, 1.0)]
		[TestCase(5.0, 5.01, 10.0)]
		[TestCase(1e6, 1.2e6, 2e6)]
		[TestCase(1e-6, 2e-6, 3e-6)]
		[TestCase(2.0, 3.0, 10.0)]
		[TestCase(2.0, 9.0, 10.0)]
		[TestCase(100.0, 101.0, 200.0)]
		public void TriangularDelay_Minutes_ShouldMatchExpectedMeanAndMode(double min, double mode, double max)
		{
			double expMean = (min + mode + max) / 3.0;

			var delay = RandomTool.Delay.Triangular.InMinutes(min, mode, max);
			GenerateSamples(delay, Selector.Minutes, (min, max));

			WithMinutes(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);

				double sampleMode = Statistics.EstimateMode(data, 100);
				sampleMode.Should().BeApproximately(mode, ToleranceFactor * (max - min));
			});
		}

		[TestCase(1.0, 5.0, 10.0)]
		[TestCase(10.0, 20.0, 30.0)]
		[TestCase(2.5, 3.0, 5.0)]
		[TestCase(1.0, 1.0, 5.0)]
		[TestCase(1.0, 4.0, 5.0)]
		[TestCase(5.0, 6.0, 10.0)]
		[TestCase(100.0, 150.0, 200.0)]
		[TestCase(2.0, 3.0, 10.0)]
		[TestCase(2.0, 9.0, 10.0)]
		[TestCase(50.0, 75.0, 100.0)]
		public void TriangularDelay_Seconds_ShouldMatchExpectedMeanAndMode(double min, double mode, double max)
		{
			double expMean = (min + mode + max) / 3.0;

			var delay = RandomTool.Delay.Triangular.InSeconds(min, mode, max);
			GenerateSamples(delay, Selector.Seconds, (min, max));

			WithSeconds(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);

				double sampleMode = Statistics.EstimateMode(data, 100);
				sampleMode.Should().BeApproximately(mode, ToleranceFactor * (max - min));
			});
		}

		[TestCase(1_000.0, 5_000.0, 10_000.0)]
		[TestCase(10_000.0, 20_000.0, 30_000.0)]
		[TestCase(2_500.0, 3_000.0, 5_000.0)]
		[TestCase(1_000.0, 1_000.0, 5_000.0)]
		[TestCase(1_000.0, 4_000.0, 5_000.0)]
		[TestCase(5_000.0, 6_000.0, 10_000.0)]
		[TestCase(100_000.0, 150_000.0, 200_000.0)]
		[TestCase(2_000.0, 3_000.0, 10_000.0)]
		[TestCase(2_000.0, 9_000.0, 10_000.0)]
		[TestCase(50_000.0, 75_000.0, 100_000.0)]
		public void TriangularDelay_Milliseconds_ShouldMatchExpectedMeanAndMode(double min, double mode, double max)
		{
			double expMean = (min + mode + max) / 3.0;

			var delay = RandomTool.Delay.Triangular.InMilliseconds(min, mode, max);
			GenerateSamples(delay, Selector.Milliseconds, (min, max));

			WithMilliseconds(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);

				double sampleMode = Statistics.EstimateMode(data, 100);
				sampleMode.Should().BeApproximately(mode, ToleranceFactor * (max - min));
			});
		}
	}
}

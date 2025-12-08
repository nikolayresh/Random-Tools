using FluentAssertions;
using RandomTools.Core;

namespace RandomTools.Tests.Delay
{
	public class TriangularDelayTests : DelayTestBase
	{
		[Test, Combinatorial]
		public void TriangularDelay_Minutes_ShouldHaveExpectedMean(
			[Values(0.5, 1.0, 1.5, 2.0)] double min,
			[Values(2.5, 3.0, 3.5, 4.0)] double max)
		{
			double mode = Math.FusedMultiplyAdd(CoreTools.NextDouble(), max - min, min);
			double expMean = (min + mode + max) / 3.0;
			
			var delay = RandomTool.Delay.Triangular.InMinutes(min, mode, max);
			GenerateSamples(delay, Select.Minutes, (min, max));

			WithMinutes(data =>
			{
				var (Mean, Variance, StandardDeviation, Count) = Statistics.AnalyzeSamples(data);
				double SEM = Statistics.StandardErrorOfMean(StandardDeviation, Count);

				double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
				Mean.Should().BeApproximately(expMean, delta);
			});
		}
	}
}

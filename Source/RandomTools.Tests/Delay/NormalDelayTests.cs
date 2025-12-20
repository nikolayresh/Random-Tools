using FluentAssertions;
using RandomTools.Core;
using RandomTools.Tests;

[TestFixture]
public class NormalDelayTests : AbstractDelayTest
{
	private const double Tolerance = 0.025;

	[Test]
	[TestCase(0.0, 0.5, -2.0, 2.0)]
	[TestCase(0.0, 1.0, -5.0, 5.0)]
	[TestCase(5.0, 1.0, 0.0, 10.0)]
	[TestCase(10.0, 2.0, 5.0, 15.0)]
	[TestCase(50.0, 10.0, 30.0, 70.0)]
	[TestCase(-5.0, 1.5, -10.0, 0.0)]
	[TestCase(0.0, 0.1, -0.5, 0.5)]
	[TestCase(100.0, 20.0, 50.0, 150.0)]
	public void NormalDelay_Seconds_Should_Have_CorrectMean_And_ZeroSkewness(double mean, double stdDev, double min, double max)
	{
		var delay = RandomTool.Delay.Normal.InSeconds(mean, stdDev, (min, max));
		GenerateSamples(delay, Selector.Seconds, (min, max));

		WithSeconds(data =>
		{
			var (Mean, _, _, SEM, Skewness, _) = Statistics.AnalyzeSamples(data);

			double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
			Mean.Should().BeApproximately(mean, delta);
			Skewness.Should().BeApproximately(0.0, Tolerance);
		});
	}

	[Test]
	[TestCase(10.0, 2.0, 0.0, 20.0)]
	[TestCase(50.0, 10.0, 30.0, 70.0)]
	[TestCase(100.0, 20.0, 50.0, 150.0)]
	[TestCase(500.0, 50.0, 400.0, 600.0)]
	[TestCase(1000.0, 100.0, 800.0, 1200.0)]
	public void NormalDelay_Milliseconds_Should_Have_CorrectMean_And_ZeroSkewness(double mean, double stdDev, double min, double max)
	{
		var delay = RandomTool.Delay.Normal.InMilliseconds(mean, stdDev, (min, max));
		GenerateSamples(delay, Selector.Milliseconds, (min, max));

		WithMilliseconds(data =>
		{
			var (Mean, _, _, SEM, Skewness, _) = Statistics.AnalyzeSamples(data);

			double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
			Mean.Should().BeApproximately(mean, delta);
			Skewness.Should().BeApproximately(0.0, Tolerance);
		});
	}

	[Test]
	[TestCase(0.5, 0.1, 0.0, 1.0)]
	[TestCase(1.0, 0.2, 0.0, 2.0)]
	[TestCase(2.0, 0.5, 1.0, 3.0)]
	[TestCase(5.0, 1.0, 3.0, 7.0)]
	[TestCase(10.0, 2.0, 5.0, 15.0)]
	public void NormalDelay_Minutes_Should_Have_CorrectMean_And_ZeroSkewness(double mean, double stdDev, double min, double max)
	{
		var delay = RandomTool.Delay.Normal.InMinutes(mean, stdDev, (min, max));
		GenerateSamples(delay, Selector.Minutes, (min, max));

		WithMinutes(data =>
		{
			var (Mean, _, _, SEM, Skewness, _) = Statistics.AnalyzeSamples(data);

			double delta = Statistics.ConfidenceDelta(ConfidenceLevel.Confidence999, SEM);
			Mean.Should().BeApproximately(mean, delta);
			Skewness.Should().BeApproximately(0.0, Tolerance);
		});
	}
}

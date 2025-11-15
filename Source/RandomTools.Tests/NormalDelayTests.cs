using RandomTools.Core;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random.Delay;
using System.Diagnostics;

namespace RandomTools.Tests
{
	[TestFixture]
	public class NormalDelayTests
	{
		[Test]
		public void TestOne()
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(20)
				.WithMaximum(50)
				.WithMean(55)
				.WithStandardDeviation(10)
				.WithTimeUnit(TimeUnit.Second);

			var random = new NormalDelay(options);
			List<TimeSpan> trials = [];

			int maxTrials = 1_000_000;
			while (maxTrials-- != 0)
			{
				var next = random.Next();
				trials.Add(next);
			}

			Debugger.Break();
		}
	}
}

using RandomTools.Core;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random.Delay;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Tests
{
	[TestFixture]
	public class ExponentialDelayTests
	{
		[Test]
		public void TestOne()
		{
			var options = new DelayOptions.Exponential()
				.WithMinimum(20)
				.WithMaximum(30)
				.WithTimeUnit(TimeUnit.Second);

			var ed = new ExponentialDelay(options);
			var trials = new List<TimeSpan> ();

			int maxTrials = 1_000_000;
			while (maxTrials-- != 0)
			{
				var next = ed.Next();
				trials.Add(next);
			}

			Debugger.Break ();
		}
	}
}

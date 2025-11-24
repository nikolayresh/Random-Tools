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
	public class TriangularDelayTests
	{
		[Test]
		public void TestThree()
		{
			var options = new DelayOptions.Bates()
				.WithMinimum(100)
				.WithMaximum(200)
				.WithSamples(6);

			var delay = new BatesDelay(options);
			var data = new List<TimeSpan>();
			for (int i = 0; i < 1_000_000; i++)
			{
				var next = delay.Next();
				data.Add(next);
			}

			var min = data.Min(x => x);
			var max = data.Max(x => x);
			Debugger.Break();
		}

		[Test]
		public void TestTwo()
		{
			var options = new DelayOptions.Arcsine()
				.WithMinimum(100)
				.WithMaximum(150)
				.WithTimeUnit(TimeUnit.Second);

			var delay = new ArcsineDelay(options);
			var data = new List<TimeSpan>();

			for (int i = 0; i < 1_000_000; i++)
			{
				var next = delay.Next();
				data.Add(next);
			}

			var min = data.Min(x => x);
			var max = data.Max(x => x);

			Debugger.Break();
		}

		[Test]
		public void TestOne()
		{
			var options = new DelayOptions.Triangular()
				.WithMinimum(100)
				.WithMaximum(200)
				.WithMode(180);

			var delay = new TriangularDelay(options);
			var data = new List<TimeSpan>();
			int i = 0;
			while (true)
			{
				var next = delay.Next();
				data.Add(next);

				if (++i > 1_000_000)
					break;
			}

			var min = data.Min(x => x);
			var max = data.Max(x => x);

			Debugger.Break();
		}
	}
}

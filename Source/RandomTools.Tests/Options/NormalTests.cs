using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random.Delay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class NormalTests
	{
		[Test]
		public void TestOne()
		{
			var options = new DelayOptions.Normal()
				.WithMean(50)
				.WithStandardDeviation(10)
				.WithMinimum(60)
				.WithMaximum(65);

			options.Validate();

			var r = new NormalDelay(options);
			r.Next();
		}
	}
}

using RandomTools.Core.Options.Delay;
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
			double p = double.NaN;

			bool isTrue = double.IsFinite(p);

			var options = new DelayOptions.Normal()
				.WithAutoFit(40, 60);

			options.Validate();
		}
	}
}

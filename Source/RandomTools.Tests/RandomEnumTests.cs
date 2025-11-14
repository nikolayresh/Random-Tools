using RandomTools.Core;
using RandomTools.Core.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Tests
{
	[TestFixture]
	public class RandomEnumTests
	{
		[Test]
		public void TestOne()
		{
			var rEnum = new RandomEnum<TimeUnit>();

			var items = new HashSet<TimeUnit>();
			while (items.Count != 3)
			{
				var next = rEnum.Next();
				items.Add(next);
			}


		}
	}
}

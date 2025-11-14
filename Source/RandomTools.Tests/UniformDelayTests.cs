using FluentAssertions;
using RandomTools.Core;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random.Delay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Tests
{
	[TestFixture]
	public class UniformDelayTests
	{
		private const int Segments = 100;
		private const int Precision = 7;
		private readonly Dictionary<Range, ulong> _stats = [];

		private double PrepareStats(double min, double max)
		{
			_stats.Clear();
			double step = (max - min) / Segments;

			for (int i = 1; i <= Segments; i++)
			{
				double segmentMin = Math.Round(min + (step * (i - 1)), Precision);
				double segmentMax = Math.Round(min + (step * i), Precision);

				_stats[new Range
				{
					Minimum = segmentMin,
					Maximum = segmentMax
				}] = 0UL;
			}

			return step;
		}

		[Test]
		[TestCase(10.0, 20.0,   1_000_000L)]
		[TestCase(10.0, 20.0,  10_000_000L)]
		public void TestOne(double min, double max, long trials)
		{
			var options = new DelayOptions.Uniform()
				.WithMinimum(min)
				.WithMaximum(max)
				.WithTimeUnit(TimeUnit.Second);

			options
				.Invoking(opt => opt.Validate())
				.Should()
				.NotThrow<OptionsValidationException>();

			PrepareStats(min, max);
			var delay = new UniformDelay(options);

			long remaining = trials;
		    while (remaining-- != 0L)
			{
				TimeSpan next = delay.Next();

				var bucket = _stats.Keys.Single(
					x => x.Minimum <= next.TotalSeconds && next.TotalSeconds < x.Maximum);

				_stats[bucket]++;
			}

			double expected = 1.0 / Segments;

			var frequencies = _stats.Values.Select(next => (double)next / trials).ToList();

			frequencies.Should()
				.AllSatisfy(x => x.Should().BeApproximately(expected, 0.01d));

		}
	}
}

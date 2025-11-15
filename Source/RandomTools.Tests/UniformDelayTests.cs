using FluentAssertions;
using RandomTools.Core;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random.Delay;
using System.Diagnostics;

namespace RandomTools.Tests
{
	[TestFixture]
	public class UniformDelayTests
	{
		private record HitsBucket(double Min, double Max) {
			public double Min { get; } = Min;
			public double Max { get; } = Max;
			public int Hits { get; set; } = 0;
		}

		private const int Segments = 100;

		private HitsBucket[]? _buckets;
		private double _step;

		[TearDown]
		public void TearDown()
		{
			_step = 0;
			if (_buckets != null)
			{
				Array.Clear(_buckets);
				_buckets = null;
			}
		}

		private void PrepareBuckets(double min, double max)
		{
			_buckets = new HitsBucket[Segments];
			_step = (max - min) / Segments;

			for (int i = 0; i < Segments; i++)
			{
				double bMin = Math.Round(min + _step * i, 10);
				double bMax = Math.Round(min + _step * (i + 1), 10);

				_buckets[i] = new HitsBucket(bMin, bMax);
			}
		}

		private int GetBucketIndex(double value, double min)
		{
			int index = (int)((value - min) / _step);
			return index < Segments ? index : Segments - 1;
		}

		[Test]
		[TestCase(10.0,  20.0,    1_000_000)]
		[TestCase(10.0,  20.0,   10_000_000)]
		[TestCase(10.0,  20.0,  100_000_000)]
		[TestCase(42.91, 58.17,   1_000_000)]
		[TestCase(42.91, 58.17,  10_000_000)]
		[TestCase(42.91, 58.17, 100_000_000)]
		public void Uniform_Distribution_Proof_On_ChiSquare(double min, double max, int trials)
		{
			var options = new DelayOptions.Uniform()
				.WithMinimum(min)
				.WithMaximum(max)
				.WithTimeUnit(TimeUnit.Second);
			 
			options
				.Invoking(opt => opt.Validate())
				.Should()
				.NotThrow<OptionsValidationException>();

			PrepareBuckets(min, max);
			var delay = new UniformDelay(options);

			for (long i = 0; i < trials; i++)
			{
				TimeSpan next = delay.Next();

				int index = GetBucketIndex(next.TotalSeconds, min);
				_buckets![index].Hits++;
			}

			double chi = Algorithms.ChiSquare(trials, _buckets.Select(x => x.Hits));
			double critical = Algorithms.CriticalChiSquare(Segments - 1);

			chi.Should().BeLessThan(critical);
		}
	}
}

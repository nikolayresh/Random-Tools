using FluentAssertions;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options;
using RandomTools.Core.Random;

namespace RandomTools.Tests
{
	[TestFixture]
	public class RandomBoolTests
	{
		[Test]
		[TestCase(0.25d,   1_000_000L,   0.01d)]
		[TestCase(0.25d,  10_000_000L,   0.01d)]
		[TestCase(0.25d, 100_000_000L,  0.001d)]
		[TestCase(0.40d,   1_000_000L,   0.01d)]
		[TestCase(0.40d,  10_000_000L,   0.01d)]
		[TestCase(0.40d, 100_000_000L,  0.001d)]
		[TestCase(0.60d,   1_000_000L,   0.01d)]
		[TestCase(0.60d,  10_000_000L,   0.01d)]
		[TestCase(0.60d, 100_000_000L,  0.001d)]
		[TestCase(0.75d,   1_000_000L,   0.01d)]
		[TestCase(0.75d,  10_000_000L,   0.01d)]
		[TestCase(0.75d, 100_000_000L,  0.001d)]
		[TestCase(0.90d,   1_000_000L,   0.01d)]
		[TestCase(0.90d,  10_000_000L,   0.01d)]
		[TestCase(0.90d, 100_000_000L,  0.001d)]
		public void Next_WithBias_Should_GenerateValues_CloseToBias(double bias, long trials, double precision)
		{
			var options = new BoolOptions().WithBias(bias);
			var randomBool = new RandomBool(options);

			long trueHits = 0;
			long remaining = trials;

			while (remaining-- != 0)
			{
				if (randomBool.Next())
					trueHits++;
			}

			double trueRatio = trueHits / (double)trials;

			trueRatio.Should().BeApproximately(bias, precision);
		}

		[Test]
		[TestCase(  1_000_000L,   0.01d)]
		[TestCase( 10_000_000L,   0.01d)]
		[TestCase(100_000_000L,  0.001d)]
		public void Next_WithoutBias_Should_Generate_Approximately_Equal_TrueFalse(long trials, double precision)
		{
			var options = new BoolOptions();
			var randomBool = new RandomBool(options);

			long trueHits = 0;
			long falseHits = 0;

			while (trials-- != 0)
			{
				if (randomBool.Next())
					trueHits++;
				else
					falseHits++;
			}

			long totalHits = trueHits + falseHits;
			double trueRatio = trueHits / (double)totalHits;
			double falseRatio = falseHits / (double)totalHits;

			// Assert: true/false ratio is approximately 0.5
			trueRatio.Should().BeApproximately(0.5d, precision);
			falseRatio.Should().BeApproximately(0.5d, precision);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void NextExcept_With_Single_ExcludedValue_Should_Throw_ArgumentException(bool excludedValue)
		{
			var options = new BoolOptions();
			var randomBool = new RandomBool(options);

			randomBool.Invoking(rb => rb.NextExcept(excludedValue))
				.Should()
				.Throw<ArgumentException>()
				.WithParameterName("excludedValues");
		}

		[Test]
		public void NextExcept_With_Multiple_ExcludedValues_Should_Throw_ArgumentException()
		{
			var options = new BoolOptions();
			var randomBool = new RandomBool(options);

			randomBool.Invoking(rb => rb.NextExcept([true, false]))
				.Should()
				.Throw<ArgumentException>()
				.WithParameterName("excludedValues");
		}

		[Test]
		public void Ctor_Should_Throw_When_Bias_Is_Zero()
		{
			Action action = () => new RandomBool(new BoolOptions().WithBias(0));

			action.Should().Throw<OptionsValidationException>()
				.Which.Message
				.Should().Contain($"Actual value: {0:G}");
		}

		[Test]
		[TestCase(-1)]
		[TestCase(-0.1)]
		[TestCase(-0.000001)]
		public void Ctor_Should_Throw_When_Bias_Is_Negative(double bias)
		{
			Action action = () => new RandomBool(new BoolOptions().WithBias(bias));

			action.Should().Throw<OptionsValidationException>()
				.Which.Message
				.Should().Contain($"Actual value: {bias:G}");
		}

		[Test]
		public void Ctor_Should_Throw_When_Bias_Is_One()
		{
			Action action = () => new RandomBool(new BoolOptions().WithBias(1));

			action.Should().Throw<OptionsValidationException>()
				.Which.Message
				.Should().Contain($"Actual value: {1:G}");
		}

		[Test]
		[TestCase(1.000001)]
		[TestCase(1.01)]
		[TestCase(2)]
		public void Ctor_Should_Throw_When_Bias_Is_GreaterThan_One(double bias)
		{
			Action action = () => new RandomBool(new BoolOptions().WithBias(bias));

			action.Should().Throw<OptionsValidationException>()
				.Which.Message
				.Should().Contain($"Actual value: {bias:G}");
		}

		[Test]
		public void Ctor_Should_Not_Throw_When_Bias_Is_NotSet()
		{
			Action action = () => new RandomBool(new BoolOptions());

			action.Should().NotThrow<OptionsValidationException>();
		}
	}
}

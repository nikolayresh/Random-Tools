using FluentAssertions;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;
using System.Security.Cryptography;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class NormalOptionsFixture
	{
		[Test]
		public void When_Unconfigured_Should_Throw_On_Validate()
		{
			var options = new DelayOptions.Normal();

			options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which.Options
				.Should().BeSameAs(options);
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_Minimum_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_Maximum_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMaximum(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_Mean_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMean(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_StandardDeviation_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithStandardDeviation(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.ZeroLike))]
		public void When_StandardDeviation_Is_ZeroLike_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithStandardDeviation(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		[TestCase(-0.1)]
		[TestCase(-0.5)]
		[TestCase(-1.0)]
		[TestCase(-10.0)]
		public void When_StandardDeviation_Is_Negative_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithStandardDeviation(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		[TestCase(0.0, 0.0)]
		[TestCase(0.0, 0.0 + double.Epsilon)]
		[TestCase(1.0, 1.0)]
		[TestCase(1.0, 1.0 + double.Epsilon)]
		[TestCase(5.0, 5.0)]
		[TestCase(5.0, 5.0 + double.Epsilon)]
		[TestCase(15.0, 15.0)]
		[TestCase(15.0, 15.0 + double.Epsilon)]
		[TestCase(85.0, 85.0)]
		[TestCase(85.0, 85.0 + double.Epsilon)]
		public void When_Range_Is_Too_Narrow_Should_Throw_On_Validate(double min, double max)
		{
			var options = new DelayOptions.Normal()
				.WithStandardDeviation(RandomNumberGenerator.GetInt32(10, 30))
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"[{min}");
			ex.Message.Should().Contain($"{max}]");
		}

		[Test]
		[TestCase(40, 10, 10)]
		[TestCase(0, 5, 3)]
		[TestCase(0, 20, 10)]
		[TestCase(0, 10, 5)]
		[TestCase(-10, 10, 20)]
		[TestCase(14.8, 5.9, 12.3)]
		[TestCase(100.5, 34.18, 12.83)]
		public void When_Range_Has_Shift_Of_Three_Sigma_ToRight_Should_Not_Throw_On_Validate(double mean, double stdDev, double rangeLength)
		{
			double rangeMin = mean + 3 * stdDev;
			double rangeMax = rangeMin + rangeLength;

			var options = new DelayOptions.Normal()
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(rangeMin)
				.WithMaximum(rangeMax);

			options.Invoking(opt => opt.Validate())
				.Should()
				.NotThrow<OptionsValidationException>();
		}

		[Test]
		[TestCase(0, 10, 5)]
		[TestCase(50, 30, 10)]
		[TestCase(10.5, 4.7, 12.6)]
		[TestCase(-10, 3, 5)]
		[TestCase(-100, 20, 15)]
		[TestCase(0.58, 7.3, 5.1)]
		public void When_Range_Has_Shift_Of_Three_Sigma_ToLeft_Should_Not_Throw_On_Validate(double mean, double stdDev, double rangeLength)
		{
			double rangeMax = mean - 3 * stdDev;
			double rangeMin = rangeMax - rangeLength;

			var options = new DelayOptions.Normal()
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(rangeMin)
				.WithMaximum(rangeMax);

			options.Invoking(opt => opt.Validate())
				.Should()
				.NotThrow<OptionsValidationException>();
		}

		[Test]
		[TestCase(0, 10, 0.001)]
		[TestCase(0, 10, 0.01)]
		[TestCase(0, 10, 0.1)]
		[TestCase(0, 10, 1)]
		[TestCase(25, 5, 0.001)]
		[TestCase(25, 5, 0.01)]
		[TestCase(25, 5, 0.1)]
		[TestCase(25, 5, 1)]
		[TestCase(-50, 10, 0.001)]
		[TestCase(-50, 10, 0.01)]
		[TestCase(-50, 10, 0.1)]
		[TestCase(-50, 10, 1)]
		public void When_Range_Has_Shift_Of_More_Than_Three_Sigma_ToLeft_Should_Throw_On_Validate(double mean, double stdDev, double delta)
		{
			const double length = 10.0;

			double rangeMax = mean - (3 * (stdDev + delta));
			double rangeMin = rangeMax - length;

			var options = new DelayOptions.Normal()
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(rangeMin)
				.WithMaximum(rangeMax);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"[{rangeMin}");
			ex.Message.Should().Contain($"{rangeMax}]");
			ex.Message.Should().Contain($"({mean})");
		}

		[Test]
		[TestCase(0, 10, 0.001)]
		[TestCase(0, 10, 0.01)]
		[TestCase(0, 10, 0.1)]
		[TestCase(0, 10, 1)]
		[TestCase(25, 5, 0.001)]
		[TestCase(25, 5, 0.01)]
		[TestCase(25, 5, 0.1)]
		[TestCase(25, 5, 1)]
		[TestCase(-50, 10, 0.001)]
		[TestCase(-50, 10, 0.01)]
		[TestCase(-50, 10, 0.1)]
		[TestCase(-50, 10, 1)]
		public void When_Range_Has_Shift_Of_More_Than_Three_Sigma_ToRight_Should_Throw_On_Validate(double mean, double stdDev, double delta)
		{
			const double length = 10.0;

			double rangeMin = mean + (3 * (stdDev + delta));
			double rangeMax = rangeMin + length;

			var options = new DelayOptions.Normal()
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(rangeMin)
				.WithMaximum(rangeMax);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"[{rangeMin}");
			ex.Message.Should().Contain($"{rangeMax}]");
			ex.Message.Should().Contain($"({mean})");
		}
	}
}

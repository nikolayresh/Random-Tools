using FluentAssertions;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class NormalOptionsFixture
	{
		[Test]
		public void When_Unconfigured_Should_Throw_On_Validate()
		{
			var options = new DelayOptions.Normal();

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().Contain(ExceptionMessages.RangeIsTooShort);
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

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Minimum, ExceptionMessages.Format(value, true));
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

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Maximum, ExceptionMessages.Format(value, true));
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_Mean_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(100.0)
				.WithMaximum(200.0)
				.WithMean(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Mean, ExceptionMessages.Format(value, true));
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_StandardDeviation_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(100.0)
				.WithMaximum(200.0)
				.WithMean(150.0)
				.WithStandardDeviation(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.StandardDeviation, ExceptionMessages.Format(value, true));
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.ZeroLike))]
		public void When_StandardDeviation_Is_ZeroLike_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(100.0)
				.WithMaximum(200.0)
				.WithMean(150.0)
				.WithStandardDeviation(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().Contain(ExceptionMessages.Format(value, true));
		}

		[Test]
		[TestCase( -0.1)]
		[TestCase( -0.5)]
		[TestCase( -1.0)]
		[TestCase(-10.0)]
		public void When_StandardDeviation_Is_Negative_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(100.0)
				.WithMaximum(200.0)
				.WithMean(150.0)
				.WithStandardDeviation(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().Contain(ExceptionMessages.Format(value, true));
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
		public void When_Range_Is_Too_Short_Should_Throw_On_Validate(double min, double max)
		{
			var options = new DelayOptions.Normal()
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				ExceptionMessages.RangeIsTooShort,
				Keywords.Minimum,
				Keywords.Maximum);
		}

		[Test]
		[TestCase(  0.0,  5.0,  50.0,  60.0)]
		[TestCase( 40.0, 10.0, 150.0, 170.0)]
		[TestCase(-25.0,  2.0,   0.0,   5.0)]
		public void When_Range_Is_Too_Far_From_Mean_Should_Throw_On_Validate(double mean, double stdDev, double min, double max)
		{
			var options = new DelayOptions.Normal()
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				ExceptionMessages.Format(mean, false),
				ExceptionMessages.Format(stdDev, false),
				ExceptionMessages.Format(min, false),
				ExceptionMessages.Format(max, false));
		}

		[Test]
		public void When_Used_As_Dictionary_Key_Should_Return_Correct_Value()
		{
			const double min = 200.0;
			const double max = 400.0;
			const double mean = 300.0;
			const double stdDev = 25.0;

			var options = new DelayOptions.Normal()
				.WithMinimum(min)
				.WithMaximum(max)
				.WithMean(mean)
				.WithStandardDeviation(stdDev);

			var keyToLookup = (DelayOptions.Normal)options.Clone();
			var expectedValue = new object();

			var dict = new Dictionary<DelayOptions.Normal, object>
			{
				[options] = expectedValue
			};

			bool exists = dict.TryGetValue(keyToLookup, out object? actualValue);

			exists.Should().BeTrue();
			actualValue.Should().Be(expectedValue);
		}

		[Test]
		[TestCase( 200.0, 800.0)]
		[TestCase( 100.0, 300.0)]
		[TestCase(   0.0, 300.0)]
		[TestCase(-300.0,   0.0)]
		public void When_AutoFit_Used_Should_Evaluate_Correct_Mean_And_StandardDeviation(double min, double max)
		{
			var options = new DelayOptions.Normal()
				.WithAutoFit(min, max);

			options.Mean.Should().Be((min + max) / 2.0);
			options.StandardDeviation.Should().Be((max - min) / 6.0);
		}
	}
}
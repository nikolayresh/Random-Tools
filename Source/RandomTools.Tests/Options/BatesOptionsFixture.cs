using FluentAssertions;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class BatesOptionsFixture
	{
		[Test]
		public void When_Unconfigured_Should_Throw_On_Validate()
		{
			var options = new DelayOptions.Bates();

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().Contain(ExceptionMessages.RangeIsTooShort);
		}

		[Test]
		public void When_Cloning_Should_Return_Equal_But_Distinct_Options()
		{
			const double min = 230.45;
			const double max = 450.80;
			const int samples = 5;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max)
				.WithSamples(samples);

			IOptionsBase? clonedOptions = null;
			Action act = () => clonedOptions = options.Clone();

			act.Should().NotThrow();
			clonedOptions.Should().NotBeNull().And.BeOfType<DelayOptions.Bates>();
			clonedOptions.Should().Be(options).And.NotBeSameAs(options);
		}

		[Test]
		[TestCaseSource(typeof(ValuesProvider), nameof(ValuesProvider.NonFinite))]
		public void When_Minimum_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Bates()
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
			var options = new DelayOptions.Bates()
				.WithMaximum(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Maximum, ExceptionMessages.Format(value, true));
		}

		[Test]
		public void When_Minimum_Is_GreaterThan_Maximum_Should_Throw_On_Validate()
		{
			const double min = 48.25;
			const double max = 30.75;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				Keywords.Minimum, Keywords.Maximum, 
				ExceptionMessages.Format(min, true),
				ExceptionMessages.Format(max, true));
		}

		[Test]
		public void When_Minimum_Equals_To_Maximum_Should_Throw_On_Validate()
		{
			const double min = 250.0;
			const double max = 250.0;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				Keywords.Minimum, Keywords.Maximum,
				ExceptionMessages.RangeIsTooShort,
				ExceptionMessages.Format(min, true),
				ExceptionMessages.Format(max, true));
		}

		[Test]
		public void When_Range_Is_Too_Short_Should_Throw_On_Validate()
		{
			const double min = 300.0;
			const double max = min + double.Epsilon;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				Keywords.Minimum, Keywords.Maximum,
				ExceptionMessages.RangeIsTooShort,
				ExceptionMessages.Format(min, true),
				ExceptionMessages.Format(max, true));
		}

		[Test]
		public void When_Samples_Is_Unset_Should_Throw_On_Validate()
		{
			const double min = 200.0;
			const double max = 450.0;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Samples, ExceptionMessages.Format(0, true));
		}

		[Test]
		public void When_Samples_Is_Negative_Should_Throw_On_Validate()
		{
			const double min = 100;
			const double max = 300;
			const int samples = -5;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max)
				.WithSamples(samples);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Samples, ExceptionMessages.Format(samples, true));
		}

		[Test]
		public void When_Used_As_Dictionary_Key_Should_Return_Correct_Value()
		{
			const double min = 150.0;
			const double max = 400.0;
			const int samples = 3;

			var options = new DelayOptions.Bates()
				.WithMinimum(min)
				.WithMaximum(max)
				.WithSamples(samples);

			var keyToLookup = (DelayOptions.Bates) options.Clone();
			var expectedValue = new object();

			var dict = new Dictionary<DelayOptions.Bates, object>
			{
				[options] = expectedValue
			};

			bool exists = dict.TryGetValue(keyToLookup, out object? actualValue);

			exists.Should().BeTrue();
			actualValue.Should().BeSameAs(expectedValue);
		}
	}
}

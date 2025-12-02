using FluentAssertions;
using RandomTools.Core.Exceptions;
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
			ex.Message.Should().ContainAll(Keywords.Minimum, ExceptionMessages.Value(value, true));
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
			ex.Message.Should().ContainAll(Keywords.Maximum, ExceptionMessages.Value(value, true));
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
			ex.Message.Should().ContainAll(Keywords.Minimum, Keywords.Maximum, 
				ExceptionMessages.Value(min, true),
				ExceptionMessages.Value(max, true));
		}
	}
}

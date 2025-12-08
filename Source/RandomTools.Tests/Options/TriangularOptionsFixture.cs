using FluentAssertions;
using RandomTools.Core;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class TriangularOptionsFixture
	{
		[Test]
		public void When_Unconfigured_Should_Throw_On_Validate()
		{
			var options = new DelayOptions.Triangular();

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
			var options = new DelayOptions.Triangular()
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
			var options = new DelayOptions.Triangular()
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
		public void When_Mode_Is_Not_Finite_Should_Throw_On_Validate(double value)
		{
			var options = new DelayOptions.Triangular()
				.WithMinimum(100.0)
				.WithMaximum(200.0)
				.WithMode(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(Keywords.Mode, ExceptionMessages.Format(value, true));
		}

		[Test]
		public void When_Mode_Is_LessThan_Minimum_Should_Throw_On_Validate()
		{
			double min = Math.Round(CoreTools.NextDouble(400.0, 800.0));
			double max = Math.Round(CoreTools.NextDouble(1200.0, 1600.0));
			double mode = Math.Round(min - 100.0);

			var options = new DelayOptions.Triangular()
				.WithTimeUnit(TimeUnit.Millisecond)
				.WithMinimum(min)
				.WithMaximum(max)
				.WithMode(mode);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				Keywords.Mode,
				ExceptionMessages.Format(mode, true),
				ExceptionMessages.Format(min, false), 
				ExceptionMessages.Format(max, false));
		}

		[Test]
		public void When_Mode_Is_GreaterThan_Maximum_Should_Throw_On_Validate()
		{
			double min = Math.Round(CoreTools.NextDouble(200.0, 500.0));
			double max = Math.Round(CoreTools.NextDouble(800.0, 1200.0));
			double mode = Math.Round(max + 100.0);

			var options = new DelayOptions.Triangular()
				.WithTimeUnit(TimeUnit.Millisecond)
				.WithMinimum(min)
				.WithMaximum(max)
				.WithMode(mode);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().ContainAll(
				Keywords.Mode,
				ExceptionMessages.Format(mode, true),
				ExceptionMessages.Format(min, false),
				ExceptionMessages.Format(max, false));
		}

		[Test]
		public void When_Used_As_Dictionary_Key_Should_Return_Correct_Value()
		{
			const double min = 850.0;
			const double max = 1450.0;
			const double mode = 1200.0;

			var options = new DelayOptions.Triangular()
				.WithTimeUnit(TimeUnit.Millisecond)
				.WithMinimum(min)
				.WithMaximum(max)
				.WithMode(mode);

			var keyToLookup = (DelayOptions.Triangular) options.Clone();
			var expectedValue = new object();

			var dict = new Dictionary<DelayOptions.Triangular, object>
			{
				[options] = expectedValue
			};

			bool exists = dict.TryGetValue(keyToLookup, out object? actualValue);

			exists.Should().BeTrue();
			actualValue.Should().Be(expectedValue);
		}

		[Test]
		[TestCase(0.0), TestCase(0.1)]
		[TestCase(0.2), TestCase(0.3)]
		[TestCase(0.4), TestCase(0.5)]
		[TestCase(0.6), TestCase(0.7)]
		[TestCase(0.8), TestCase(0.9)]
		[TestCase(1.0)]
		public void When_Valid_Options_Provided_Should_Not_Throw_On_Validate(double modeFactor)
		{
			double min = Math.Round(CoreTools.NextDouble(600.0, 900.0));
			double max = Math.Round(CoreTools.NextDouble(1200.0, 1600.0));
			double mode = Math.FusedMultiplyAdd(modeFactor, max - min, min);

			var options = new DelayOptions.Triangular()
				.WithTimeUnit(TimeUnit.Millisecond)
				.WithMinimum(min)
				.WithMaximum(max)
				.WithMode(mode);

			options.Invoking(opt => opt.Validate())
				.Should()
				.NotThrow<OptionsValidationException>();
		}
	}
}

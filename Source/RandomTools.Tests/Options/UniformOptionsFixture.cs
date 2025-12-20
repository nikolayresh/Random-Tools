using FluentAssertions;
using RandomTools.Core;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class UniformOptionsFixture
	{
		private readonly Dictionary<DelayOptions.Uniform, object> _dict = new();

		[Test]
		public void When_Unconfigured_Should_Throw_On_Validate()
		{
			var options = new DelayOptions.Uniform();

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
			var options = new DelayOptions.Uniform()
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
			var options = new DelayOptions.Uniform()
				.WithMaximum(value);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().Contain(ExceptionMessages.Format(value, true));
		}

		[Test]
		public void When_Minimum_Is_Greater_Than_Maximum_Should_Throw_On_Validate()
		{
			double minimum = 200;
			double maximum = 100;

			var options = new DelayOptions.Uniform()
				.WithMinimum(minimum)
				.WithMaximum(maximum);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().Be(options).And.NotBeSameAs(options);
			ex.Message.Should().Contain($"({minimum})");
			ex.Message.Should().Contain($"({maximum})");
		}

		[Test]
		[TestCase(0.0, 0.0)]
		[TestCase(0.0, 0.0 + double.Epsilon)]
		[TestCase(1.0, 1.0)]
		[TestCase(1.0, 1.0 + double.Epsilon)]
		[TestCase(100.0, 100.0)]
		[TestCase(100.0, 100.0 + double.Epsilon)]
		[TestCase(-1.0, -1.0)]
		[TestCase(-1.0, -1.0 + double.Epsilon)]
		[TestCase(-15.0, -15.0)]
		[TestCase(-15.0, -15.0 + double.Epsilon)]
		public void When_Range_Is_Too_Narrow_Should_Throw_On_Validate(double minimum, double maximum)
		{
			var options = new DelayOptions.Uniform()
				.WithMinimum(minimum)
				.WithMaximum(maximum);

			options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which.Options
				.Should().BeSameAs(options);
		}

		[Test]
		[TestCase( 10.0,  20.0, TimeUnit.Millisecond)]
		[TestCase( 10.0,  20.0, TimeUnit.Second)]
		[TestCase( 10.0,  20.0, TimeUnit.Minute)]
		[TestCase(    0,  10.0, TimeUnit.Millisecond)]
		[TestCase(    0,  10.0, TimeUnit.Second)]
		[TestCase(    0,  10.0, TimeUnit.Minute)]
		[TestCase(16.47, 84.92, TimeUnit.Millisecond)]
		[TestCase(16.47, 84.92, TimeUnit.Second)]
		[TestCase(16.47, 84.92, TimeUnit.Minute)]
		public void Equality_And_HashCode_Should_Work_For_Equivalent_Uniform_Options(double minimum, double maximum, TimeUnit unit)
		{
			var options = new DelayOptions.Uniform()
				.WithTimeUnit(unit)
				.WithMinimum(minimum)
				.WithMaximum(maximum);

			_dict.Clear();
			_dict[options] = new object();

            var optionsKey = new DelayOptions.Uniform()
				.WithTimeUnit(unit)
				.WithMinimum(minimum)
				.WithMaximum(maximum);

			bool exists = _dict.TryGetValue(optionsKey, out _);
			exists.Should().BeTrue();
		}
	}
}

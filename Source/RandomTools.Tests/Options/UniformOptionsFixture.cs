using FluentAssertions;
using RandomTools.Core.Exceptions;
using RandomTools.Core.Options.Delay;

namespace RandomTools.Tests.Options
{
	[TestFixture]
	public class UniformOptionsFixture
	{
		[Test]
		public void When_Unconfigured_Should_Not_Throw_On_Validate()
		{
			var options = new DelayOptions.Uniform();

			options.Invoking(opt => opt.Validate())
				.Should()
				.NotThrow<OptionsValidationException>();
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

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
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

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({value})");
		}

		[Test]
		public void When_Minimum_Is_Greater_Than_Maximum_Should_Throw_On_Validate()
		{
			double minimum, maximum;

			do
			{
				minimum = Math.Round(1_000 * Random.Shared.NextDouble());
				// wrong decrement on purpose
				maximum = Math.Round(minimum - 10.0);
			} while (minimum < 128d);

			var options = new DelayOptions.Uniform()
				.WithMinimum(minimum)
				.WithMaximum(maximum);

			var ex = options.Invoking(opt => opt.Validate())
				.Should()
				.Throw<OptionsValidationException>()
				.Which;

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"({minimum})");
			ex.Message.Should().Contain($"({maximum})");
		}
	}
}

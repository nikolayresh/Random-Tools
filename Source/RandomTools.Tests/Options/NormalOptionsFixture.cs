using FluentAssertions;
using RandomTools.Core;
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
		[TestCase( -0.1)]
		[TestCase( -0.5)]
		[TestCase( -1.0)]
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
				.WithMean(0.0)
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

			ex.Options.Should().BeSameAs(options);
			ex.Message.Should().Contain($"[{min}");
			ex.Message.Should().Contain($"{max}]");
		}

		[Test]
		public void Equals_Returns_True_For_Same_NormalOptions()
		{
			const double mean = 10.0;
			const double stdDev = 5.0;
			const double min = 2.0;
			const double max = 18.0;
			const TimeUnit unit = TimeUnit.Second;

			var optX = new DelayOptions.Normal()
				.WithTimeUnit(unit)
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(min)
				.WithMaximum(max);

			var optY = new DelayOptions.Normal()
				.WithTimeUnit(unit)
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(min)
				.WithMaximum(max);

			optX.Equals(optY).Should().BeTrue();
		}

		[Test]
		public void NormalOptions_Can_Be_Used_As_Dictionary_Key()
		{
			const double mean = 0.0;
			const double stdDev = 5.0;
			const double min = -10.0;
			const double max = 10.0;
			const TimeUnit unit = TimeUnit.Second;
			Guid expectedId = Guid.NewGuid();

			var dict = new Dictionary<DelayOptions.Normal, Guid>();
			var options = new DelayOptions.Normal()
				.WithTimeUnit(unit)
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(min)
				.WithMaximum(max);

			dict[options] = expectedId;

			var lookupKey = new DelayOptions.Normal()
				.WithTimeUnit(unit)
				.WithMean(mean)
				.WithStandardDeviation(stdDev)
				.WithMinimum(min)
				.WithMaximum(max);

			bool found = dict.TryGetValue(lookupKey, out Guid actualId);
			
			found.Should().BeTrue();
			actualId.Should().Be(expectedId);
		}
	}
}

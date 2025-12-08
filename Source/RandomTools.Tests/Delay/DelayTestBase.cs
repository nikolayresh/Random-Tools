using FluentAssertions;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random.Delay;

/// <summary>
/// Base class for testing delay generators.
/// <para>
/// Provides common utilities for generating large samples of <see cref="TimeSpan"/> delays,
/// converting them to different units, and performing statistical checks in tests.
/// </para>
/// </summary>
public abstract class DelayTestBase
{
	/// <summary>
	/// Common selectors to convert <see cref="TimeSpan"/> to numerical values.
	/// <para>
	/// Used to analyze delays in different units (milliseconds, seconds, minutes).
	/// </para>
	/// </summary>
	protected static class Select
	{
		/// <summary>Converts a <see cref="TimeSpan"/> to milliseconds.</summary>
		public static readonly Func<TimeSpan, double> Milliseconds = ts => ts.TotalMilliseconds;

		/// <summary>Converts a <see cref="TimeSpan"/> to seconds.</summary>
		public static readonly Func<TimeSpan, double> Seconds = ts => ts.TotalSeconds;

		/// <summary>Converts a <see cref="TimeSpan"/> to minutes.</summary>
		public static readonly Func<TimeSpan, double> Minutes = ts => ts.TotalMinutes;
	}

	/// <summary>
	/// Number of iterations used to generate sample delays in tests.
	/// </summary>
	protected const int Iterations = 100_000;

	/// <summary>
	/// Stores all generated <see cref="TimeSpan"/> delays for the current test.
	/// </summary>
	protected List<TimeSpan> _delays;

	/// <summary>
	/// Test setup method.
	/// <para>
	/// Initializes or clears the internal list of delays before each test.
	/// </para>
	/// </summary>
	[SetUp]
	public virtual void OnSetUp()
	{
		_delays ??= new List<TimeSpan>();
		_delays.Clear();
	}

	/// <summary>
	/// Generates a large number of sample delays from the specified generator.
	/// <para>
	/// Each generated value is checked to be within the given bounds.
	/// All samples are stored in <see cref="_delays"/> for further analysis.
	/// </para>
	/// </summary>
	/// <typeparam name="TOptions">The type of options used by the generator.</typeparam>
	/// <param name="generator">The delay generator to produce samples. Cannot be null.</param>
	/// <param name="selector">Function to convert <see cref="TimeSpan"/> to a numeric value for analysis. Cannot be null.</param>
	/// <param name="bounds">Expected minimum and maximum values of generated delays.</param>
	protected void GenerateSamples<TOptions>(
		RandomDelay<TOptions> generator,
		Func<TimeSpan, double> selector,
		(double Min, double Max) bounds)
		where TOptions : DelayOptionsBase<TOptions>
	{
		ArgumentNullException.ThrowIfNull(generator);
		ArgumentNullException.ThrowIfNull(selector);

		for (int i = 0; i < Iterations; i++)
		{
			TimeSpan next = generator.Next();
			selector.Invoke(next)
				.Should()
				.BeInRange(bounds.Min, bounds.Max);

			_delays.Add(next);
		}
	}

	/// <summary>
	/// Executes an action on all generated delays converted to milliseconds.
	/// </summary>
	/// <param name="action">Action to perform on the numeric values.</param>
	protected void WithMilliseconds(Action<IEnumerable<double>> action)
	{
		ArgumentNullException.ThrowIfNull(action);

		action.Invoke(_delays.Select(Select.Milliseconds));
	}

	/// <summary>
	/// Executes an action on all generated delays converted to seconds.
	/// </summary>
	/// <param name="action">Action to perform on the numeric values.</param>
	protected void WithSeconds(Action<IEnumerable<double>> action)
	{
		ArgumentNullException.ThrowIfNull(action);

		action.Invoke(_delays.Select(Select.Seconds));
	}

	/// <summary>
	/// Executes an action on all generated delays converted to minutes.
	/// </summary>
	/// <param name="action">Action to perform on the numeric values.</param>
	protected void WithMinutes(Action<IEnumerable<double>> action)
	{
		ArgumentNullException.ThrowIfNull(action);

		action.Invoke(_delays.Select(Select.Minutes));
	}
}

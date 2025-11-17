using RandomTools.Core.Options;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random;
using RandomTools.Core.Random.Delay;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace RandomTools.Core
{
	/// <summary>
	/// Set of useful random tools
	/// </summary>
	public static class RandomTool
	{
		public static class Text
		{
			private readonly static ConcurrentDictionary<TextOptions, RandomText> sCache = new();

			public static RandomText UniqueLetters(int length)
			{
				var options = new TextOptions()
					.UseLowerLetters()
					.UseUpperLetters()
					.UseLength(length)
					.Unique();

				return GetEntry(options);
			}

			public static RandomText Letters(int length)
			{
				var options = new TextOptions()
					.UseLowerLetters()
					.UseUpperLetters()
					.UseLength(length);

				return GetEntry(options);
			}

			public static RandomText UniqueDigits(int length)
			{
				var options = new TextOptions()
					.UseDigits()
					.UseLength(length)
					.Unique();

				return GetEntry(options);
			}

			public static RandomText Digits(int length)
			{
				var options = new TextOptions()
					.UseDigits()
					.UseLength(length);

				return GetEntry(options);
			}

			private static RandomText GetEntry(TextOptions options) => sCache.GetOrAdd(options, _ => new RandomText(options));
		}

		/// <summary>
		/// Provides easy access to <see cref="RandomBool"/> instances, 
		/// including a shared default instance and cached biased instances.
		/// </summary>
		public static class Bool
		{
			// Shared instance that generates unbiased true/false values
			private static readonly RandomBool sInstance = new();

			// Cache of instances keyed by their BoolOptions (with bias)
			private static readonly ConcurrentDictionary<BoolOptions, RandomBool> sBiasCache = new();

			/// <summary>
			/// Gets a shared default <see cref="RandomBool"/> instance (50/50 probability)
			/// </summary>
			public static RandomBool Instance => sInstance;

			/// <summary>
			/// Returns a random boolean value as a pure coin flip
			/// </summary>
			public static bool Next() => sInstance.Next();

			/// <summary>
			/// Gets a <see cref="RandomBool"/> instance with the specified bias.
			/// Instances are cached for reuse.
			/// </summary>
			/// <param name="bias">Probability of returning <c>true</c>, in the exclusive range (0.0 - 1.0).</param>
			/// <returns>A <see cref="RandomBool"/> instance configured with the specified bias.</returns>
			public static RandomBool Bias(double bias)
			{
				var options = new BoolOptions().WithBias(bias);

				return sBiasCache.GetOrAdd(options,
					_ => new RandomBool(options));
			}
		}

		public static class Delay
		{
			/// <summary>
			/// Provides methods to create delays with a uniform random distribution.
			/// <para>
			/// Uniform delays generate a random duration between a minimum and maximum value.
			/// This is useful for simulating non-deterministic delays in testing, retries, or throttling.
			/// </para>
			/// <para>
			/// Delays are cached to avoid repeatedly creating identical UniformDelay instances.
			/// This improves performance and reduces memory allocations when using repeated delay ranges.
			/// </para>
			/// </summary>
			public static class Uniform
			{
				/// <summary>
				/// Cache for UniformDelay instances.
				/// <para>
				/// Keyed by DelayOptions to ensure that each unique min/max/time unit combination
				/// only creates a single UniformDelay object. Thread-safe via ConcurrentDictionary.
				/// </para>
				/// </summary>
				private static readonly ConcurrentDictionary<DelayOptions.Uniform, UniformDelay> sCache = new();

				/// <summary>
				/// Creates or retrieves a cached UniformDelay with a random duration between the specified minimum and maximum in milliseconds.
				/// <para>
				/// Example: Millisecond(100, 500) will randomly pick a delay between 100ms and 500ms.
				/// </para>
				/// </summary>
				/// <param name="minimum">The minimum delay value in milliseconds.</param>
				/// <param name="maximum">The maximum delay value in milliseconds.</param>
				/// <returns>A UniformDelay instance representing the requested random delay range.</returns>
				public static UniformDelay InMilliseconds(double minimum, double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Millisecond)
						.WithMinimum(minimum)
						.WithMaximum(maximum);

					// Retrieve existing delay from cache, or create a new one if it doesn't exist.
					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				/// <summary>
				/// Creates or retrieves a cached UniformDelay with a random duration between 0 and the specified maximum in milliseconds.
				/// <para>
				/// A convenience overload for when the minimum is always 0.
				/// Example: Millisecond(200) will randomly pick a delay between 0ms and 200ms.
				/// </para>
				/// </summary>
				/// <param name="maximum">The maximum delay in milliseconds.</param>
				/// <returns>A UniformDelay instance for the requested range.</returns>
				public static UniformDelay InMilliseconds(double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Millisecond)
						.WithMinimum(0)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay InSeconds(double minimum, double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Second)
						.WithMinimum(minimum)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay InSeconds(double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Second)
						.WithMinimum(0)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay InMinutes(double minimum, double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Minute)
						.WithMinimum(minimum)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay InMinutes(double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Minute)
						.WithMinimum(0)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}
			}

			/// <summary>
			/// Provides factory methods for constructing <see cref="NormalDelay"/> instances,
			/// which generate random delays using a truncated normal (Gaussian) distribution.
			/// <para>
			/// A truncated normal distribution is a standard normal curve (defined by mean and
			/// standard deviation) that is restricted to a finite interval [Minimum, Maximum].
			/// Values falling outside this interval are discarded and regenerated using
			/// rejection sampling, ensuring that every produced delay is guaranteed to satisfy
			/// the configured bounds.
			/// </para>
			/// 
			/// <h2>Purpose</h2>
			/// <para>
			/// This class provides an ergonomic, high-level API for creating Gaussian-based
			/// delays with common patterns such as:
			///   • Explicit mean and standard deviation  
			///   • Auto-fitting a natural-looking distribution to a range  
			///   • Choosing the time unit (ms / s / min)  
			///   • Reusing identical delay generators via internal caching  
			/// </para>
			/// 
			/// <h2>Caching</h2>
			/// <para>
			/// All methods in this class return cached <see cref="NormalDelay"/> instances.
			/// The cache key is the immutable <see cref="DelayOptions.Normal"/> configuration.
			/// If two calls use identical parameters (time unit, mean, deviation, min/max,  
			/// or AutoFit mode), the same delay generator instance is returned.
			/// This avoids unnecessary object creation and ensures consistent behavior
			/// across repeated operations.
			/// </para>
			/// 
			/// <h2>When to Use Normal Delays</h2>
			/// <para>
			/// Normal delays are appropriate when you want delays to cluster near a preferred
			/// central value (the mean), while still allowing some variability. Common use cases:
			///   • Simulating human-like timing (typing, UI interactions, behavior modeling)  
			///   • Adding jitter to network retries or backoff loops  
			///   • Creating natural distributions of wait times in testing  
			///   • Modeling real-world events that follow a bell curve  
			/// </para>
			/// 
			/// <h2>AutoFit vs Explicit Parameters</h2>
			/// <para>
			/// <b>AutoFit</b> is a convenience mode where the library computes a reasonable mean
			/// and deviation based on the specified range. This is useful for "I just want it to
			/// look natural" scenarios.
			/// </para>
			/// <para>
			/// The <b>explicit</b> overloads (Mean + StandardDeviation + Range) provide complete
			/// control and are suitable when the shape of the Gaussian curve must match a
			/// statistical model or real data.
			/// </para>
			/// 
			/// <h2>Thread Safety</h2>
			/// <para>
			/// All factory methods are thread-safe. Cached instances are stored in a
			/// <see cref="ConcurrentDictionary{TKey,TValue}"/> and may be safely retrieved
			/// from multiple threads concurrently. The returned <see cref="NormalDelay"/>
			/// instances themselves are also thread-safe for concurrent use.
			/// </para>
			/// </summary>
			public static class Normal
			{
				/// <summary>
				/// Cache of <see cref="NormalDelay"/> instances keyed by their
				/// <see cref="DelayOptions.Normal"/> configuration.
				/// Since <see cref="DelayOptions.Normal"/> is immutable and implements correct
				/// value-based equality, it can safely serve as a dictionary key.
				/// Two calls with identical parameters will return the same shared NormalDelay
				/// instance. This avoids redundant allocations and ensures consistent behavior.
				/// </para>
				/// <para>
				/// The dictionary is thread-safe and safe for concurrent access.
				/// </para>
				/// </summary>
				private static readonly ConcurrentDictionary<DelayOptions.Normal, NormalDelay> sCache = new();

				/// <summary>
				/// Creates (or retrieves from cache) a <see cref="NormalDelay"/> instance
				/// whose mean and standard deviation are automatically computed so that
				/// the resulting truncated normal distribution "fits" naturally into the
				/// provided range <paramref name="min"/>–<paramref name="max"/>.
				/// <para>
				/// AutoFit is intended for scenarios where you want a natural-looking
				/// Gaussian spread over a known interval but do not want to manually compute
				/// mean and deviation. It places the mean roughly in the center and uses a
				/// deviation proportional to the interval size.
				/// </para>
				/// <para>
				/// All values are still strictly clamped to the interval using rejection
				/// sampling. AutoFit only influences the underlying normal curve—not the
				/// boundaries.
				/// </para>
				/// </summary>
				/// <param name="min">Lower bound of allowed delay values.</param>
				/// <param name="max">Upper bound of allowed delay values.</param>
				/// <param name="unit">The time unit (ms/s/min) for this delay generator.</param>
				/// <returns>A cached or newly created <see cref="NormalDelay"/> instance.</returns>
				public static NormalDelay AutoFit(double min, double max, TimeUnit unit)
				{
					var options = new DelayOptions.Normal()
						.WithTimeUnit(unit)
						.WithAutoFit(min, max);

					return sCache.GetOrAdd(options,
						_ => new NormalDelay(options));
				}

				/// <summary>
				/// Convenience overload for <see cref="AutoFit(double,double,TimeUnit)"/>  
				/// using <see cref="TimeUnit.Millisecond"/>.
				/// </summary>
				public static NormalDelay AutoFitInMilliseconds(double min, double max) =>
					AutoFit(min, max, TimeUnit.Millisecond);

				/// <summary>
				/// Convenience overload for <see cref="AutoFit(double,double,TimeUnit)"/>  
				/// using <see cref="TimeUnit.Second"/>.
				/// </summary>
				public static NormalDelay AutoFitInSeconds(double min, double max) =>
					AutoFit(min, max, TimeUnit.Second);

				/// <summary>
				/// Convenience overload for <see cref="AutoFit(double,double,TimeUnit)"/>  
				/// using <see cref="TimeUnit.Minute"/>.
				/// </summary>
				public static NormalDelay AutoFitInMinutes(double min, double max) =>
					AutoFit(min, max, TimeUnit.Minute);

				/// <summary>
				/// Creates (or retrieves from cache) a <see cref="NormalDelay"/> configured
				/// with an explicit mean and standard deviation, using milliseconds as the
				/// time unit.
				/// <para>
				/// Values are produced from N(mean, stdDev) but are guaranteed to fall within
				/// the inclusive interval [min, max] via rejection sampling. If too many
				/// values fall outside the range, generation may take additional attempts,
				/// but never produces an out-of-range result.
				/// </para>
				/// <para>
				/// This method is intended when you want full control over the Gaussian curve,
				/// such as matching measured timing distributions, simulation models, or
				/// controlled jitter profiles.
				/// </para>
				/// </summary>
				public static NormalDelay InMilliseconds(double mean, double stdDev, double min, double max)
				{
					var options = new DelayOptions.Normal()
						.WithTimeUnit(TimeUnit.Millisecond)
						.WithMean(mean)
						.WithStandardDeviation(stdDev)
						.WithMinimum(min)
						.WithMaximum(max);

					return sCache.GetOrAdd(options,
						_ => new NormalDelay(options));
				}

				/// <summary>
				/// Same as <see cref="InMilliseconds(double,double,double,double)"/>  
				/// but uses seconds as the time unit.
				/// </summary>
				public static NormalDelay InSeconds(double mean, double stdDev, double min, double max)
				{
					var options = new DelayOptions.Normal()
						.WithTimeUnit(TimeUnit.Second)
						.WithMean(mean)
						.WithStandardDeviation(stdDev)
						.WithMinimum(min)
						.WithMaximum(max);

					return sCache.GetOrAdd(options,
						_ => new NormalDelay(options));
				}

				/// <summary>
				/// Same as <see cref="InMilliseconds(double,double,double,double)"/>  
				/// but uses minutes as the time unit.
				/// </summary>
				public static NormalDelay InMinutes(double mean, double stdDev, double min, double max)
				{
					var options = new DelayOptions.Normal()
						.WithTimeUnit(TimeUnit.Minute)
						.WithMean(mean)
						.WithStandardDeviation(stdDev)
						.WithMinimum(min)
						.WithMaximum(max);

					return sCache.GetOrAdd(options,
						_ => new NormalDelay(options));
				}

				/// <summary>
				/// Tuple-based overload for defining the allowed range of delay values.
				/// Semantically identical to calling:
				/// <c>InMilliseconds(mean, stdDev, range.Min, range.Max)</c>
				/// </summary>
				public static NormalDelay InMilliseconds(double mean, double stdDev, (double Min, double Max) range) =>
					InMilliseconds(mean, stdDev, range.Min, range.Max);

				/// <summary>
				/// Tuple-based overload for seconds.
				/// </summary>
				public static NormalDelay InSeconds(double mean, double stdDev, (double Min, double Max) range) =>
					InSeconds(mean, stdDev, range.Min, range.Max);

				/// <summary>
				/// Tuple-based overload for minutes.
				/// </summary>
				public static NormalDelay InMinutes(double mean, double stdDev, (double Min, double Max) range) =>
					InMinutes(mean, stdDev, range.Min, range.Max);
			}
		}
	}
}
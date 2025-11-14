using RandomTools.Core.Options;
using RandomTools.Core.Options.Delay;
using RandomTools.Core.Random;
using RandomTools.Core.Random.Delay;
using System.Collections.Concurrent;

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
				public static UniformDelay Millisecond(double minimum, double maximum)
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
				public static UniformDelay Millisecond(double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Millisecond)
						.WithMinimum(0)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options, 
						_ => new UniformDelay(options));
				}

				public static UniformDelay Second(double minimum, double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Second)
						.WithMinimum(minimum)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay Second(double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Second)
						.WithMinimum(0)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay Minute(double minimum, double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Minute)
						.WithMinimum(minimum)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				}

				public static UniformDelay Minute(double maximum)
				{
					var options = new DelayOptions.Uniform()
						.WithTimeUnit(TimeUnit.Minute)
						.WithMinimum(0)
						.WithMaximum(maximum);

					return sCache.GetOrAdd(options,
						_ => new UniformDelay(options));
				} 
			}
		}
	}
}
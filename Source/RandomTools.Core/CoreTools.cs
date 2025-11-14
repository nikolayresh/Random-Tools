using System.Security.Cryptography;

namespace RandomTools.Core
{
	/// <summary>
	/// Provides core utilities for cryptographically secure random number generation
	/// and time conversion operations.
	/// </summary>
	internal static class CoreTools
	{
		// Mask to isolate the lower 52 bits (mantissa/fraction) of a double.
		// Using all 52 bits gives exactly 2^52 distinct values,
		// allowing uniform coverage of the [0.0, 1.0) range when
		// constructing doubles directly from bits.
		private const ulong MantissaMask = 0x000F_FFFF_FFFF_FFFFUL;

		// Exponent mask corresponding to 1023 (biased) in IEEE 754.
		// OR'ing a mantissa with this mask produces a normalized
		// double in the [1.0, 2.0) range. Subtracting 1.0
		// afterwards maps it to [0.0, 1.0) without bias.
		private const ulong ExponentMask = 0x3FF0_0000_0000_0000UL;
		
		/// <summary>
		/// Converts a numeric value to a <see cref="TimeSpan"/> according to a specified <see cref="TimeUnit"/>.
		/// </summary>
		/// <param name="value">The numeric value to convert.</param>
		/// <param name="unit">The time unit of <paramref name="value"/>.</param>
		/// <returns>A <see cref="TimeSpan"/> representing the same duration.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="unit"/> is not Millisecond, Second, or Minute.
		/// </exception>
		public static TimeSpan ToTimeSpan(double value, TimeUnit unit) => unit switch
		{
			TimeUnit.Millisecond => TimeSpan.FromMilliseconds(value),
			TimeUnit.Second => TimeSpan.FromSeconds(value),
			TimeUnit.Minute => TimeSpan.FromMinutes(value),

			_ => throw new ArgumentOutOfRangeException(nameof(unit), unit,
				"Unsupported TimeUnit. Supported units: Millisecond, Second, Minute.")
		};

		/// <summary>
		/// Generates a cryptographically secure random byte.
		/// </summary>
		/// <returns>A random byte in the range [0, 255].</returns>
		public static byte RandomByte()
		{
			Span<byte> buffer = stackalloc byte[1];
			RandomNumberGenerator.Fill(buffer);

			return buffer[0];
		}

		/// <summary>
		/// Generates a cryptographically secure random double within the range
		/// [<paramref name="min"/>, <paramref name="max"/>).
		/// </summary>
		/// <param name="min">The inclusive lower bound of the range.</param>
		/// <param name="max">
		/// The exclusive upper bound of the range.  
		/// If <paramref name="min"/> equals <paramref name="max"/>, the value of <paramref name="min"/> is returned.
		/// </param>
		/// <returns>
		/// A uniformly distributed double in the interval [<paramref name="min"/>, <paramref name="max"/>),
		/// or <paramref name="min"/> when the bounds are equal.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="min"/> is greater than <paramref name="max"/>.
		/// </exception>
		public static double NextDouble(double min, double max)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max);

			// Identical bounds — return the bound value
			if (min == max)
				return min;

			// Scale to the target range [min, max)
			return min + ((max - min) * NextDouble());
		}

		/// <summary>
		/// Generates a cryptographically secure random double in the range [0.0, 1.0).
		/// </summary>
		/// <returns>A secure random double ≥ 0.0 and &lt; 1.0.</returns>
		public static double NextDouble()
		{
			Span<byte> buffer = stackalloc byte[8];
			RandomNumberGenerator.Fill(buffer);

			ulong mantissa = BitConverter.ToUInt64(buffer) & MantissaMask;

			// Map to [1.0, 2.0) then subtract 1.0 → [0.0, 1.0)
			return -1.0 + BitConverter.UInt64BitsToDouble(mantissa | ExponentMask);
		}
	}
}

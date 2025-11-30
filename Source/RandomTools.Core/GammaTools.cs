using RandomTools.Core;

/// <summary>
/// Provides utility methods for generating random values from the Gamma distribution.
/// </summary>
internal static class GammaTools
{
	/// <summary>
	/// Generates a random value from the Gamma distribution with the specified shape and scale.
	/// </summary>
	/// <param name="shape">Shape parameter α (> 0)</param>
	/// <param name="scale">Scale parameter θ (> 0)</param>
	/// <returns>A random value sampled from Gamma(α, θ)</returns>
	public static double NextGamma(double shape, double scale)
	{
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(shape, 0.0);
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(scale, 0.0);

		// case: shape < 1.0
		if (shape < 1.0)
		{
			double u = CoreTools.NextDouble();
			return NextGamma(shape + 1.0, scale) * Math.Pow(u, 1.0 / shape);
		}

		// case: shape >= 1.0
		return scale * MarsagliaTsang(shape);
	}

	/// <summary>
	/// Marsaglia–Tsang Gamma sampler for shape ≥ 1 (Gamma(shape, 1))
	/// </summary>
	/// <param name="shape">Shape parameter α (≥ 1)</param>
	/// <returns>A random value sampled from Gamma(shape, 1)</returns>
	private static double MarsagliaTsang(double shape)
	{
		// Precompute constants
		double d = shape - (1.0 / 3.0);
		double c = 1.0 / Math.Sqrt(9.0 * d);

		while (true)
		{
			double x, v;

			do
			{
				// Standard Normal ~ N(0,1)
				x = GaussianTools.NextNormal();
				v = 1.0 + (c * x);
			} while (v <= 0.0);

			v = v * v * v;
			// Uniform Random ~ [0,1)
			double u = CoreTools.NextDouble();

			// Fast acceptance condition (approximation)
			if (u < 1.0 - (0.331 * (x * x) * (x * x)))
				return d * v;

			// Full acceptance condition (exact)
			if (Math.Log(u) < (-0.5 * x * x) + (d * (1.0 - v + Math.Log(v))))
				return d * v;
		}
	}
}

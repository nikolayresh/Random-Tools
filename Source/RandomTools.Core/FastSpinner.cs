namespace RandomTools.Core
{
	/// <summary>
	/// A cyclic spinner that increases spin intensity using <c>1 << _count</c>,
	/// automatically wrapping to zero when the threshold is reached.
	/// </summary>
	internal sealed class FastSpinner
	{
		private readonly int _threshold;
		private int _count;

		/// <summary>
		/// Initializes a new instance of <see cref="FastSpinner"/>.
		/// </summary>
		/// <param name="threshold">
		/// Maximum counter value before wrapping. Must be between 1 and 30
		/// to avoid integer overflow with <c>1 << _count</c>.
		/// </param>
		public FastSpinner(int threshold = 10)
		{
			if (threshold <= 0 || threshold > 30)
			{
				throw new ArgumentOutOfRangeException(nameof(threshold),
					"Threshold must be between 1 and 30 to prevent overflow with 1 << _count");
			}

			_threshold = threshold;
			_count = 0;
		}

		/// <summary>
		/// Performs one spin iteration using <c>1 << _count</c>,
		/// then increments the counter and wraps it back to 0 if it exceeds the threshold.
		/// </summary>
		public void SpinOnce()
		{
			Thread.SpinWait(1 << _count);

			_count++;
			// Reset counter when it hits the threshold
			if (_count >= _threshold)
				_count = 0;
		}

		/// <summary>
		/// Resets the spinner counter to 0.
		/// </summary>
		public void Reset()
		{
			_count = 0;
		}
	}
}

using RandomTools.Core.Options;

namespace RandomTools.Core.Random
{
	public abstract class RandomBase<TValue, TOptions> where TOptions: IOptionsBase
	{
		protected readonly TOptions Options;

		protected RandomBase(TOptions options)
		{
			ArgumentNullException.ThrowIfNull(options);
			options.Validate();
		
			Options = options;
		}

		public abstract TValue Next();

		public TValue NextExcept(Func<TValue, bool> excludeFn)
		{
			ArgumentNullException.ThrowIfNull(excludeFn);

			TValue next;

			do
			{
				next = Next();
			}
			while (excludeFn.Invoke(next));

			return next;
		}

		public TValue NextExcept(TValue excluded) => NextExcept([excluded]);

		public virtual TValue NextExcept(IEnumerable<TValue> excludedValues)
		{
			ArgumentNullException.ThrowIfNull(excludedValues);

			var excludedSet = new HashSet<TValue>(excludedValues);
			if (excludedSet.Count == 0)
				return Next();

			TValue next;

			do
			{
				next = Next();
			} while (excludedSet.Contains(next));

			return next;
		}
	}
}

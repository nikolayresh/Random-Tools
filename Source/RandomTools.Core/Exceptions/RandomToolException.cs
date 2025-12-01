using RandomTools.Core.Options;

namespace RandomTools.Core.Exceptions
{
	public abstract class RandomToolException : Exception
	{
		protected const string AppName = "Random-Tools";

#pragma warning disable IDE0290
		protected RandomToolException(IOptionsBase options, string? message) : base(message)
#pragma warning restore IDE0290
		{
			Options = options;
		}

		public IOptionsBase Options { get; }
	}
}

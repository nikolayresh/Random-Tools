using RandomTools.Core.Options;

namespace RandomTools.Core.Exceptions
{
	public sealed class NextGeneratorException : RandomToolException
	{
#pragma warning disable IDE0290
		public NextGeneratorException(IOptionsBase options, string? message) 
			: base(options, message) { }
#pragma warning restore IDE0290
	}
}

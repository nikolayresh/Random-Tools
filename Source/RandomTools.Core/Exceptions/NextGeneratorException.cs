using RandomTools.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Core.Exceptions
{
	public class NextGeneratorException : RandomToolException
	{
		public NextGeneratorException(IOptionsBase options, string? message) : base(options, message)
		{
		}
	}
}

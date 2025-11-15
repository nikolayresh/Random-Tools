using RandomTools.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Core.Exceptions
{
	public abstract class RandomToolException : Exception
	{
		protected const string AppName = "Random-Tools";

		protected RandomToolException(IOptionsBase options, string? message) : base(message) 
		{
			Options = options;
		}

		public IOptionsBase Options { get; }
	}
}

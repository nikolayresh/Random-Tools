using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTools.Tests
{
	internal class Range
	{
		public double Minimum { get; set; }
		public double Maximum { get; set; }

		public override bool Equals(object? obj)
		{
			if (obj is null || obj.GetType() != typeof(Range))
			{
				return false;
			}

			var other = (Range)obj;

			return other.Minimum == Minimum && 
				other.Maximum == Maximum;
		}

		public override string ToString() => $"[{Minimum} - {Maximum}]";
		public override int GetHashCode() => HashCode.Combine(Minimum, Maximum);
	}
}

using System.Collections.Generic;
using Orange.Library.Values;

namespace Orange.Library
{
	public interface IReplaceBlocks
	{
		IEnumerable<Block> Blocks
		{
			get;
			set;
		}
	}
}
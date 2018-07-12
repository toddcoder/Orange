using Orange.Library.Values;

namespace Orange.Library
{
	public class RegionBlock
	{
		public RegionBlock(Region region, Block block)
		{
			Region = region;
			Block = block;
		}

		public Region Region
		{
			get;
			set;
		}

		public Block Block
		{
			get;
			set;
		}
	}
}
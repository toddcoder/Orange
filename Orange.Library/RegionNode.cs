namespace Orange.Library
{
	public class RegionNode
	{
		Region region;
		RegionNode parent;

		public RegionNode(Region region = null, RegionNode parent = null)
		{
			this.region = region ?? new Region();
			this.parent = parent;
		}

		public Region Region => region;

	   public RegionNode Parent
		{
			get => parent;
		   set => parent = value;
		}
	}
}
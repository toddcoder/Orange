namespace Orange.Library
{
	public class GraphIndexer
	{
		int index;

		public GraphIndexer() => index = 0;

	   public override string ToString() => "$" + index++;
	}
}
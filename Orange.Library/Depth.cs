using static Orange.Library.Runtime;

namespace Orange.Library
{
	public class Depth
	{
		int maxDepth;
		string location;
		int depth;

		public Depth(int maxDepth, string location)
		{
			this.maxDepth = maxDepth;
			this.location = location;
			depth = 0;
		}

		public void Retain(string message) => Assert(++depth <= maxDepth, location, message);

	   public void Retain() => Retain($"Maximum depth of {maxDepth} exceeded");

	   public void Release() => Assert(--depth > -1, location, "Internal error: unbalanced depth release");

	   public void Reset() => depth = 0;

	   public int Level => depth;
	}
}
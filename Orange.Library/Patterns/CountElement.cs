using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class CountElement : Element
	{
		int count;
		Pattern pattern;

		public CountElement(int count, Pattern pattern)
		{
			this.count = count;
			this.pattern = pattern;
			this.pattern.SubPattern = true;
		}

		public override bool Evaluate(string input)
		{
			index = -1;
			var anchored = State.Anchored;
			State.Anchored = true;
		   for (var i = 0; i < count; i++)
		      if (pattern.Scan(input))
		      {
		         if (index == -1)
		            index = pattern.Index;
		      }
		      else
		      {
		         State.Anchored = anchored;
		         return false;
		      }
		   State.Anchored = anchored;
			length = State.Position - index;
			return true;
		}

		public override bool PositionAlreadyUpdated => true;

	   public override string ToString() => $"{count}({pattern})";

	   public override Element Clone() => clone(new CountElement(count, (Pattern)pattern.Clone()));
	}
}
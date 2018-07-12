using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class FieldScanElement : Element
	{
		int count;

		public FieldScanElement(int count) => this.count = count;

	   public override bool Evaluate(string input) => find(input, State.Position);

	   bool find(string input, int start)
		{
			if (count == 0)
				return false;

			if (input.Length == 0)
			{
				if (State.Multi)
					return false;
				index = State.Position;
				length = 0;
				return true;
			}

			var pattern = State.FieldPattern;
			var at = start;
			var segment = 0;
			for (var i = 0; i < count; i++)
			{
				at = pattern.Find(input, at + segment, out segment);
				if (at == -1)
					return false;
			}
			index = State.Position;
			length = at - index + segment;
			return true;
		}

		public override string ToString() => count + "/";

	   public override Element Clone() => new FieldScanElement(count)
	   {
	      Next = cloneNext(),
	      Alternate = cloneAlternate(),
	      Replacement = cloneReplacement(),
	   };
	}
}
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class AssertElement : Element
	{
		protected string text;

		public AssertElement(string text) => this.text = text;

	   public override bool Evaluate(string input)
		{
			index = State.Position;
			length = text.Length;
			var haystack = input.Skip(index).Take(length);
			var compare = string.Compare(haystack, text, State.IgnoreCase);
			if (Not)
			{
				if (index == 0 || index >= input.Length)
				{
					length = 0;
					return true;
				}
				return compare != 0;
			}
			if (index == 0 || index >= input.Length)
			{
				length = 0;
				return false;
			}
			return compare == 0;
		}

		public override Element Clone() => new AssertElement(text.Copy());
	}
}
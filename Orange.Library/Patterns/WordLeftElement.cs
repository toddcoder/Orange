using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class WordLeftElement : Element
	{
		public override bool Evaluate(string input) => Not ? !isBound(input) : isBound(input);

	   public override Element Clone() => clone(new WordLeftElement());

	   bool isBound(string input)
		{
			index = State.Position;
			if (index == 0 || index >= input.Length)
			{
				length = 0;
				return true;
			}
			var position = index - 1;

			var pattern = input.Skip(index).Take(1).IsMatch("/w") ? "-/w" : "/w";
			var isMatch = input.Skip(position).Take(1).IsMatch(pattern);
			length = 0;
			return isMatch;
		}

		public override string ToString() => "<<";
	}
}
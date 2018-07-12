using static System.StringComparison;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class StringElement : Element
	{
		public static bool EvaluateText(bool ignoreCase, string input, string text, bool not, ref int index, ref int length,
         int start)
		{
			var textLength = text.Length;
			var inputLength = input.Length;

			if (textLength + start > inputLength)
			{
				index = start;
				length = 0;
				return not;
			}

			if (textLength == 0)
			{
				index = start;
				length = textLength;
				return true;
			}

			if (start >= inputLength)
			{
				index = start;
				length = 0;
				return not;
			}

			if (string.Compare(text, 0, input, start, textLength, ignoreCase) == 0)
			{
				if (not)
					return false;
				index = start;
				length = textLength;
				return true;
			}
			if (not)
			{
				index = start;
				length = 0;
				return true;
			}
			return false;
		}

		protected string text;

		public StringElement(string text) => this.text = text;

	   public override bool EvaluateFirst(string input)
		{
			if (Not)
				return Evaluate(input);

			var comparison = State.IgnoreCase ? OrdinalIgnoreCase : Ordinal;
			if (State.Position >= input.Length)
				return false;
			var foundIndex = input.IndexOf(text, State.Position, comparison);
			if (foundIndex == -1)
				return false;
			index = foundIndex;
			length = text.Length;
			return true;
		}

		public override bool Evaluate(string input) => EvaluateText(State.IgnoreCase, input, text, Not, ref index,
         ref length, State.Position);

	   public override Element Clone() => new StringElement(text)
		{
		   Next = cloneNext(),
		   Alternate = cloneAlternate(),
		   Replacement = cloneReplacement(),
		   ID = CompilerState.ObjectID()
		};

	   public override string ToString() => $"'{text.Replace("'", "`'")}'";
	}
}
using Orange.Library.Replacements;
using static System.Array;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class BreakElement : Element
	{
		protected string text;
		protected string needle;
		protected char[] needleChars;

		public BreakElement(string text)
		{
			this.text = Expand(text);
			needle = null;
			needleChars = null;
		}

		public BreakElement()
			: this("")
		{
		}

		public override bool Evaluate(string input)
		{
			input = FixText(text, input, ref needle, ref needleChars);

			var start = -1;
			var stop = -1;
			var inputLength = input.Length;
			for (var i = State.Position; i < inputLength; i++)
				if (IndexOf(needleChars, input[i]) == -1)
				{
					if (start == -1)
						start = i;
					stop = i;
				}
				else
					break;
			if (start == -1)
				return false;
			if (stop == -1)
				stop = inputLength - 1;
			index = start;
			length = stop - start + 1;
			return true;
		}

		public override bool EvaluateFirst(string input)
		{
			input = FixText(text, input, ref needle, ref needleChars);

			var start = -1;
			var stop = -1;
			var inputLength = input.Length;
			for (var i = State.Position; i < inputLength; i++)
				if (IndexOf(needleChars, input[i]) == -1)
				{
					if (start == -1)
						start = i;
					stop = i;
				}
				else if (start > -1)
					break;
			if (start == -1)
				return false;
			if (stop == -1)
				stop = inputLength - 1;
			index = start;
			length = stop - start + 1;
			return true;
		}

		public override string ToString() => $"-('{needle}')";

	   public override Element Clone() => clone(new BreakElement(text));

      public override IReplacement Replacement
      {
         get => replacement;
         set => setOverridenReplacement(value);
      }


   }
}
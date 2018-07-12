using Orange.Library.Replacements;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class SpanElement : Element
	{
		protected string text;
		protected string needle;
		protected char[] needleChars;

		public SpanElement(string text) => this.text = Expand(text);

	   public SpanElement()
			: this("")
		{
		}

		public override bool Evaluate(string input)
		{
			var comparison = SetUpSearchText(text, State.IgnoreCase, ref needle);
			index = State.Position;
			var start = index;
			return Find(input, comparison, start, needle, Not, ref index, ref length);
		}

		public override bool EvaluateFirst(string input)
		{
			var comparison = SetUpSearchText(text, State.IgnoreCase, ref needle);
			if (State.IgnoreCase)
				needle = needle.ToLower();
			if (needleChars == null)
				needleChars = needle.ToCharArray();
			index = State.Position;
			return Find(input, comparison, index, needle, Not, ref index, ref length);
		}

		public override Element Clone() => clone(new SpanElement(text));

	   public override string ToString() => $"+('{needle}')";

      public override IReplacement Replacement
      {
         get => replacement;
         set => setOverridenReplacement(value);
      }
   }
}
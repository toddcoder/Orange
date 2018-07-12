using Standard.Types.Strings;
using static System.StringComparison;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class MSpanElement : SpanElement
	{
		protected int count;

		public MSpanElement(string text, int count = -1)
			: base(text) => this.count = count;

	   public MSpanElement()
			: this("")
		{
		}

		public override bool Evaluate(string input)
		{
			var comparison = State.IgnoreCase ? OrdinalIgnoreCase : Ordinal;
			if (count == -1)
				count = text.Length;
			if (needle == null)
				needle = Expand(text);

			index = State.Position;
			for (var i = index; i < count; i++)
			{
				var character = input.Skip(i).Take(1);
				if (character == "")
					return false;
			   if(!InText(needle, character, comparison))
				{
					length = i - index;
					return length > 0;
				}
			}
			length = count - index;
			return length > 0;
		}

		public override Element Alternate
		{
			get
			{
				if (count == -1)
					count = State.Input.Length;
				if (count == 1)
					return alternate;
				if (needle == null)
					needle = Expand(text);
				return new MSpanElement(needle, count - 1)
				{
					Alternate = alternate,
					Next = next
				};
			}
			set => alternate = value;
		}

		public override string ToString() => $"++{{'{needle}'}}";

	   public override Element Clone() => clone(new MSpanElement(text, count));
   }
}
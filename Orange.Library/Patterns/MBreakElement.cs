using System;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class MBreakElement : BreakElement
	{
		protected int count;

		public MBreakElement(string text, int count = -1)
			: base(text) => this.count = count;

	   public MBreakElement()
			: this("")
		{
		}

		public override bool Evaluate(string input)
		{
			var comparison = State.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			if (count == -1)
				count = text.Length;
			if (needle == null)
				needle = Expand(text);

			index = State.Position;
			for (var i = index; i < count; i++)
			{
				var character = text.Skip(i).Take(1);
				if (character == "")
					return false;
			   if (InText(needle, character, comparison))
				{
					length = i - index;
					return length > -1;
				}
			}
			length = count - index;
			return length > 0;
		}

		public override string ToString() => $"--{{'{needle}'}}";

	   public override Element Clone() => clone(new MBreakElement(text, count));

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
				return new MBreakElement(needle, count - 1)
				{
					Alternate = alternate,
					Next = next
				};
			}
			set => alternate = value;
	   }
	}
}
using Orange.Library.Replacements;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class MArbElement : Element
	{
		public MArbElement(int length = -1) => this.length = length;

	   public override bool Evaluate(string input)
		{
			index = State.Position;
			return length == 0 || index + length < input.Length;
		}

		public override void Initialize()
		{
			if (length == -1)
				length = State.Input.Length - State.Position;
		}

		public override string ToString() => "**";

	   public override Element Alternate
		{
			get
			{
				if (length == 0)
					return alternate;
				return new MArbElement(length - 1)
				{
					Next = next,
					Replacement = Replacement,
					Alternate = alternate
				};
			}
			set => base.Alternate = value;
	   }

		public override Element Clone() => clone(new MArbElement());

      public override IReplacement Replacement
      {
         get => replacement;
         set => setOverridenReplacement(value);
      }
   }
}
using System;
using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Patterns
{
	public class BlockElement2 : Element
	{
		Block block;
		bool positionAlreadyUpdated;

		public BlockElement2(Block block) => this.block = block;

	   public override bool PositionAlreadyUpdated => positionAlreadyUpdated;

	   public override bool Evaluate(string input) => evaluate(e => e.Evaluate(input), p => p.Scan(input));

	   public override bool EvaluateFirst(string input) => evaluate(e => e.EvaluateFirst(input), p => p.Scan(input));

	   bool evaluate(Func<Element, bool> func, Func<Pattern, bool> scanFunc)
		{
			positionAlreadyUpdated = false;
			index = -1;
			length = 0;

			var value = block.Evaluate();
			string text;
			StringElement element;
			bool result;
	      replacement = replacement?.Clone();
	      switch (value.Type)
			{
				case Value.ValueType.Pattern:
					var pattern = (Pattern)value;
					var anchored = State.Anchored;
					State.Anchored = true;
					pattern.OwnerNext = next;
					pattern.OwnerReplacement = pattern.Replacement?.Clone();
					pattern.SubPattern = true;
					if (scanFunc(pattern))
					{
						index = pattern.Index;
						length = pattern.Length;
						positionAlreadyUpdated = true;
						State.Anchored = anchored;
						return true;
					}
					State.Anchored = anchored;
					break;
				case Value.ValueType.Array:
					foreach (var item in (Array)value)
					{
						text = item.Value.Text;
						if (text.IsNotEmpty())
						{
							element = new StringElement(text);
							result = func(element);
							if (result)
							{
								index = element.Index;
								length = element.Length;
								return true;
							}
						}
					}
					return false;
				default:
					text = value.Text;
					if (text == null)
						return false;
					element = new StringElement(text);
					result = func(element);
					if (result)
					{
						index = element.Index;
						length = element.Length;
					}
					return result;
			}
			return false;
		}

		public override Element Clone() => clone(new BlockElement2((Block)block.Clone()));

	   public override string ToString() => $"{{{block}}}";
	}
}
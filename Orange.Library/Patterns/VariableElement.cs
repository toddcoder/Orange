using System;
using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Patterns
{
	public class VariableElement : Element
	{
		string variableName;
		bool positionAlreadyUpdated;
		Depth depth;

		public VariableElement(string variableName)
		{
			this.variableName = variableName;
			depth = new Depth(MAX_VAR_DEPTH, "Variable element");
		}

		public VariableElement()
			: this("")
		{
		}

		public override bool PositionAlreadyUpdated => positionAlreadyUpdated;

	   bool evaluate(Func<Element, bool> func, Func<Pattern, bool> scanFunc)
		{
			positionAlreadyUpdated = false;
			index = -1;
			length = 0;

			var value = RegionManager.Regions[variableName];
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
					depth.Retain($"Variable element recursion limit of {MAX_VAR_DEPTH} exceeded");
					if (scanFunc(pattern))
					{
						if (Not)
							return false;
						index = pattern.Index;
						length = pattern.Length;
						positionAlreadyUpdated = true;
						State.Anchored = anchored;
						depth.Reset();
						return true;
					}
					if (Not)
					{
						positionAlreadyUpdated = true;
						State.Anchored = anchored;
						depth.Reset();
						return true;
					}
					depth.Reset();
					State.Anchored = anchored;
					break;
				case Value.ValueType.Array:
					foreach (var item in (Array)value)
					{
						text = item.Value.Text;

						if (text.IsEmpty())
							continue;

						element = new StringElement(text);
						result = func(element);

						if (!result)
							continue;

						index = element.Index;
						length = element.Length;
						return true;
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

		public override bool Evaluate(string input) => evaluate(e => e.Evaluate(input), p => p.Scan(input));

	   public override bool EvaluateFirst(string input) => evaluate(e => e.EvaluateFirst(input), p => p.Scan(input));

	   public override string ToString() => variableName;

	   public override Element Clone() => clone(new VariableElement(variableName));
	}
}
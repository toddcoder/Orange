using System;
using Orange.Library.Managers;
using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
	public class BreakVariableElement : Element
	{
		string variableName;
		bool positionAlreadyUpdated;
		Depth depth;

		public BreakVariableElement(string variableName)
		{
			this.variableName = variableName;
			depth = new Depth(MAX_VAR_DEPTH, "Variable element");
		}

		public BreakVariableElement()
			: this("")
		{
		}

		public override bool PositionAlreadyUpdated => positionAlreadyUpdated;

	   bool evaluate(Func<Pattern, bool> scanFunc)
		{
			positionAlreadyUpdated = false;
			index = -1;
			length = 0;

			var value = RegionManager.Regions[variableName];
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
					depth.Retain();
					var inputLength = State.Input.Length;
					index = State.Position;
					length = 0;
					while (index < inputLength && !scanFunc(pattern))
					{
						State.Position++;
						positionAlreadyUpdated = true;
					}
					depth.Reset();
					State.Anchored = anchored;
					length = State.Position - index;
					return State.Position < inputLength;
			}
			return false;
		}

		public override bool Evaluate(string input) => evaluate(p => p.Scan(input));

	   public override bool EvaluateFirst(string input) => evaluate(p => p.Scan(input));

	   public override string ToString() => variableName;

	   public override Element Clone() => clone(new BreakVariableElement(variableName));
	}
}
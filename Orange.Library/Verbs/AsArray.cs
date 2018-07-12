using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class AsArray : Verb
	{
		public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, "As array");
			switch (value.Type)
			{
				case Value.ValueType.String:
					return ((String)value).ToArray();
				case Value.ValueType.Number:
					return ((Double)value).Range();
				case Value.ValueType.Array:
					return ((Array)value).Flatten();
				default:
					if (value is ISequenceSource source)
						return source.Array;
					if (value.IsArray)
						return value.SourceArray;
					return value;
			}
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Increment;

	   public override string ToString() => "!";

	   public override int OperandCount => 1;
	}
}
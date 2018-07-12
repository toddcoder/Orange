using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;
using static Orange.Library.Verbs.Verb.AffinityType;

namespace Orange.Library.Verbs
{
	public class PreDecrement : Verb
	{
		const string LOCATION = "Predecrement";

		public static Value GetPredecessor(Value value) => SendMessage(value, "pred");

	   public override Value Evaluate()
		{
			var value = State.Stack.Pop(false, LOCATION);
			if (value.IsVariable)
			{
				var variable = (Variable)value;
				value = variable.Value;
				var predecessor = GetPredecessor(value);
				if (value.Type != ValueType.Object)
					variable.Value = predecessor;
				return variable;
			}
			return GetPredecessor(value);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.PreDecrement;

	   public override string ToString() => "--";

	   public override AffinityType Affinity => Prefix;

	   public override int OperandCount => 1;
	}
}
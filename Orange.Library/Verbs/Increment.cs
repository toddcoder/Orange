using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;
using static Orange.Library.Verbs.PreIncrement;
using static Orange.Library.Verbs.Verb.AffinityType;

namespace Orange.Library.Verbs
{
	public class Increment : Verb
	{
		const string LOCATION = "Increment";

		public override Value Evaluate()
		{
			var value = State.Stack.Pop(false, LOCATION);
			if (value.IsVariable)
			{
				var variable = (Variable)value;
				value = variable.Value;
				var successor = GetSuccessor(value);
				if (value.Type != ValueType.Object)
					variable.Value = successor;
				return value;
			}
			return GetSuccessor(value);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Increment;

	   public override string ToString() => "++";

	   public override AffinityType Affinity => Postfix;

	   public override int OperandCount => 1;
	}
}
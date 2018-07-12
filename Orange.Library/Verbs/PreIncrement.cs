using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;
using static Orange.Library.Verbs.Verb.AffinityType;

namespace Orange.Library.Verbs
{
	public class PreIncrement : Verb
	{
		const string LOCATION = "Preincrement";

		public static Value GetSuccessor(Value value) => MessagingState.SendMessage(value, "succ", new Arguments());

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
				return variable;
			}
			return GetSuccessor(value);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.PreIncrement;

	   public override string ToString() => "++";

	   public override AffinityType Affinity => Prefix;

	   public override int OperandCount => 1;
	}
}
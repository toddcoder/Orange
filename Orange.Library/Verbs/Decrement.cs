using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;
using static Orange.Library.Verbs.PreDecrement;
using static Orange.Library.Verbs.Verb.AffinityType;

namespace Orange.Library.Verbs
{
	public class Decrement : Verb
	{
		const string LOCATION = "Decrement";

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
				return value;
			}
			return GetPredecessor(value);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Decrement;

	   public override string ToString() => "--";

	   public override AffinityType Affinity => Postfix;
	}
}
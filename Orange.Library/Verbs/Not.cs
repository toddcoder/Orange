using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class Not : Verb
	{
		public override Value Evaluate()
		{
			var x = State.Stack.Pop(true, "Not");
			switch (x.Type)
			{
				case ValueType.Class:
					return ((Class)x).Required();
				case ValueType.Object:
					return SendMessage(x, "not");
				case ValueType.Comprehension:
					return x.AlternateValue("not");
				default:
					return !x.IsTrue;
			}
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Not;

	   public override int OperandCount => 1;

	   public override AffinityType Affinity => AffinityType.Prefix;

	   public override string ToString() => "not";
	}
}
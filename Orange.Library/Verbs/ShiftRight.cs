using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class ShiftRight : TwoValueVerb
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.ShiftRight;

	   public override Value Evaluate(Value x, Value y) => (int)x.Number >> (int)y.Number;

	   public override string Location => "Shift right";

	   public override string Message => "shr";

	   public override string ToString() => ">>";
	}
}
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class ShiftLeft : TwoValueVerb
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.ShiftLeft;

	   public override Value Evaluate(Value x, Value y) => (int)x.Number << (int)y.Number;

	   public override string Location => "Shift left";

	   public override string Message => "shl";

	   public override string ToString() => "<<";
	}
}
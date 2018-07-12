using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class SafeDivide : TwoValueVerb
	{
		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Divide;

	   public override Value Evaluate(Value x, Value y) => y.Number == 0 ? 0 : x.Number / y.Number;

	   public override string Location => "Safe Divide";

	   public override string Message => "sdiv";

	   public override string ToString() => "//";
	}
}
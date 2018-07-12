using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class XOr : TwoValueVerb
	{
		public override Value Evaluate(Value x, Value y) => x.IsTrue ^ y.IsTrue;

	   public override string Location => "XOr";

	   public override string Message => "xor";

	   public override string ToString() => "xor";

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Or;
	}
}
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Max : TwoValueVerb
	{
		public override Value Evaluate(Value x, Value y) => x.Compare(y) < 0 ? y : x;

	   public override string Location => "Max";

	   public override string Message => "max";

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.GreaterThan;

	   public override string ToString() => "max";
	}
}
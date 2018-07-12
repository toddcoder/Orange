using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class NumericComparison : TwoValueVerb
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.Equals;

	   public override Value Evaluate(Value x, Value y) => x.Compare(y);

	   public override string Location => "Numeric comparison";

	   public override string Message => "cmp";

	   public override string ToString() => "<=>";
	}
}
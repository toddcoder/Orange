using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Divide : TwoValueVerb
	{
		public override Value Evaluate(Value x, Value y)
		{
			var divisor = y.Number;
			Reject(divisor == 0, Location, "Divide by 0");
			return x.Number / divisor;
		}

		public override string Location => "Divide";

	   public override string Message => "div";

	   public override string ToString() => "/";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Divide;
	}
}
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class PlainLessThan : ComparisonVerb
	{
		Value x;
		Value y;

		public PlainLessThan(Value x, Value y)
		{
			this.x = x;
			this.y = y;
		}

		public override Value Evaluate() => DoComparison(x, y);

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.LessThan;

	   public override bool Compare(int comparison) => comparison < 0;

	   public override string Location => "plain less than";
	}
}
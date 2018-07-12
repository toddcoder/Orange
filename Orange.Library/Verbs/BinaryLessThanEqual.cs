using Orange.Library.Managers;

namespace Orange.Library.Verbs
{
	public class BinaryLessThanEqual : ComparisonVerb
	{
		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.LessThanEqual;

	   public override bool Compare(int comparison) => comparison <= 0;

	   public override string Location => "Less than equal";

	   public override string ToString() => "<=";
	}
}
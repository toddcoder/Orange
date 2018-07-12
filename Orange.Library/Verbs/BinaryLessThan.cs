using Orange.Library.Managers;

namespace Orange.Library.Verbs
{
	public class BinaryLessThan : ComparisonVerb
	{
		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.LessThan;

	   public override bool Compare(int comparison) => comparison < 0;

	   public override string Location => "Less than";

	   public override string ToString() => "<";
	}
}
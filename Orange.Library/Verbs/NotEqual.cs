using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class NotEqual : ComparisonVerb
	{
		public override bool Compare(int comparison) => comparison != 0;

	   public override string ToString() => "!=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.NotEqual;

	   public override string Location => "Not equal";
	}
}
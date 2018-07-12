using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class MatchResult : Verb
	{
		const string STR_LOCATION = "Match result";

		public override Value Evaluate() => null;

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

	   public override string ToString() => "<|";
	}
}
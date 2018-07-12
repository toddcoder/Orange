using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class IsVal : TwoValueVerb
	{
		public override Value Evaluate(Value x, Value y) => MessageManager.MessagingState.SendMessage(x, "val?", new Arguments(y));

	   public override string Location => "Is value";

	   public override string Message => "is-val";

	   public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Equals;
	}
}
using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class IsKey : TwoValueVerb
	{
		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Equals;

	   public override Value Evaluate(Value x, Value y) => MessageManager.MessagingState.SendMessage(x, "key?", new Arguments(y));

	   public override string Location => "Is key";

	   public override string Message => "is-key";
	}
}
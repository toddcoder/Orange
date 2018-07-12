using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
	public class Repeat : TwoValueVerb
	{
		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Repeat;

	   public override Value Evaluate(Value x, Value y) => MessageManager.MessagingState.SendMessage(x, Message, new Arguments(y));

	   public override string Location => "Repeat";

	   public override string Message => "repeat";

	   public override string ToString() => "><";

	   public override bool UseArrayVersion => false;
	}
}
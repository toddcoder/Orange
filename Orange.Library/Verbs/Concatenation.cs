using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;

namespace Orange.Library.Verbs
{
	public class Concatenation : TwoValueVerb
	{
		const string LOCATION = "Concatenation";

		public override Value Evaluate(Value x, Value y)
		{
		   var generator = x.PossibleIndexGenerator();
		   var arguments = new Arguments(y);
		   return generator.FlatMap(g =>
		   {
		      var list = new GeneratorList();
		      list.Add(g);
		      return MessagingState.SendMessage(list, "concat", arguments);
		   }, () => MessagingState.SendMessage(x, "concat", arguments));
		}

	   public override string Location => "Concatenation";

	   public override string Message => "concat";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Concatenate;

	   public override string ToString() => "~";

	   public override bool UseArrayVersion => false;
	}
}
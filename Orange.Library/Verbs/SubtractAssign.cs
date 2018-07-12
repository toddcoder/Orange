using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class SubtractAssign : OperatorAssign
	{
		public override Value Execute(Variable variable, Value value) => variable.Number - value.Number;

	   public override string Location => "Subtract assign";

	   public override string Message => "sub";

	   public override string ToString() => "-=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
	}
}
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class ModAssign : OperatorAssign
	{
		public override Value Execute(Variable variable, Value value) => variable.Number % value.Number;

	   public override string Location => "Modulo assign";

	   public override string Message => "mod";

	   public override string ToString() => @"\=";

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
	}
}
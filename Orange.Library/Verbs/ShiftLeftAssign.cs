using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class ShiftLeftAssign : OperatorAssign
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

	   public override Value Execute(Variable variable, Value value) => (int)variable.Number << (int)value.Number;

	   public override string Location => "Shift left assign";

	   public override string Message => "shleft";

	   public override string ToString() => "<<=";
	}
}
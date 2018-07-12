using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class BitAndAssign : OperatorAssign
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

	   public override Value Execute(Variable variable, Value value)
		{
			return variable.Type == ValueType.Number &&
            value.Type == ValueType.Number ? (int)variable.Number & (int)value.Number :
				BitOperationOnText(variable.Value, value, (a, b) => a & b);
		}

		public override string Location => "Bit and assign";

	   public override string Message => "band";

	   public override string ToString() => "&=";
	}
}
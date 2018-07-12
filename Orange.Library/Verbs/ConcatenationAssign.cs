using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
	public class ConcatenationAssign : OperatorAssign
	{
		public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

	   public override Value Execute(Variable variable, Value value)
		{
			var variableValue = variable.Value;
			if (variableValue.IsArray)
				variableValue = variableValue.SourceArray;
			if (value.IsArray)
				value = value.SourceArray;
			if (variableValue.Type == ValueType.Array && value.Type == ValueType.Array)
			{
				var xArray = (Array)variableValue;
				var yArray = (Array)value.Resolve();
				var array = Array.Concatenate(xArray, yArray);
				variable.Value = array;
				return variable;
			}
			var text = variable.Value.Text;
			text += value.Text;
			variable.Value = text;
			return variable;
		}

		public override string Location => "Concatenation assign";

	   public override string Message => "concat";

	   public override string ToString() => "~=";

	   public override bool UseArrayVersion => false;
	}
}
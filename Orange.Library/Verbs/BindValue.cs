using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class BindValue : Verb
	{
		string variableName;
		Value value;

		public BindValue(string variableName, Value value)
		{
			this.variableName = variableName;
			this.value = value;
		}

		public override Value Evaluate()
		{
			Value resolvedValue;
			if (value is Block block)
			{
				block.AutoRegister = false;
				resolvedValue = block.Evaluate().AssignmentValue();
			}
			else
				resolvedValue = value.AssignmentValue();
		   return new BoundValue(variableName, resolvedValue);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => $"({variableName} := {value}";
	}
}
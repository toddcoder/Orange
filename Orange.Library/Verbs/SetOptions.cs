using Orange.Library.Values;
using Standard.Types.Enumerables;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class SetOptions : Verb
	{
		string[] options;

		public SetOptions(string[] options) => this.options = options;

	   public override Value Evaluate()
		{
			var value = State.Stack.Pop(true, "Set options");
			value.SetOptions(options);
			return value;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Increment;

	   public override string ToString() => $":[{options.Listify(" ")}]";
	}
}
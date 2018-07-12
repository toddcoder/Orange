using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
	public class InvokePlaceholder : Verb
	{
		string functionName;
		List<Verb> verbs;

		public InvokePlaceholder(string functionName, List<Verb> verbs)
		{
			this.functionName = functionName;
			this.verbs = verbs;
		}

		public string FunctionName => functionName;

	   public List<Verb> Verbs => verbs;

	   public Value CurrentValue
		{
			get;
			set;
		}

		public override Value Evaluate() => null;

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => $"<{functionName}({verbs.Listify(" ")})>";
	}
}
using Orange.Library.Managers;
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager.VerbPresidenceType;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class CreateExtender : Verb
	{
		const string LOCATION = "Create extender";

		string className;
		Class builder;

		public CreateExtender(string className, Class builder)
		{
			this.className = className;
			this.builder = builder;
		}

		public override Value Evaluate()
		{
			var obj = builder.NewObject(new Arguments());
			foreach (var item in obj.AllPublicInvokeables)
				State.SetExtender(className, item.Key, item.Value);
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => Statement;
	}
}
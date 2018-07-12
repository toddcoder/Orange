using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class Reference : Verb
	{
		const string LOCATION = "Reference";

	   string fieldName;

	   public Reference(string fieldName)
	   {
	      this.fieldName = fieldName;
	   }

	   public override Value Evaluate() => fieldName;

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => $"&{fieldName}";
	}
}
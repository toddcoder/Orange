using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
	public class IsDefined : Verb
	{
		string variableName;

		public IsDefined(string variableName) => this.variableName = variableName;

	   public IsDefined()
			: this("")
		{
		}

		public override Value Evaluate() => Regions.VariableExists(variableName) ? Regions[variableName] : new Nil();

	   public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

	   public override string ToString() => $"?{variableName}";
	}
}
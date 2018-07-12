using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Values.Object;

namespace Orange.Library.Verbs
{
	public class Define : Verb
	{
		string variableName;
		VisibilityType visibility;
		bool readOnly;

		public Define(string variableName, VisibilityType visibility, bool readOnly = false)
		{
			this.variableName = variableName;
			this.visibility = visibility;
			this.readOnly = readOnly;
		}

		public Define()
		{
			variableName = "";
			visibility = VisibilityType.Public;
			readOnly = false;
		}

		public override Value Evaluate() => new Variable(variableName);

	   Value evaluate(Region region)
		{
			if (readOnly)
				region.CreateReadOnlyVariable(variableName, visibility: visibility);
			else if (region.ContainsMessage(variableName))
				region.FlagExistingVariable(variableName, visibility);
			else
				region.CreateVariable(variableName, visibility: visibility);
			return new Variable(variableName);
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.PreIncrement;

	   public string VariableName => variableName;

	   public VisibilityType Visibility => visibility;

	   public bool ReadOnly => readOnly;

	   public override string ToString() => $"{(readOnly ? "val" : "var")} {variableName}";
	}
}
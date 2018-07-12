using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
	public class CreateMacro : Verb
	{
		string macroName;
		ParameterBlock parameterBlock;

		public CreateMacro(string macroName, ParameterBlock parameterBlock)
		{
			this.macroName = macroName;
			this.parameterBlock = parameterBlock;
		}

		public override Value Evaluate()
		{
			Reject(Regions.VariableExists(macroName), "Create macro", $"Macro name {macroName} already exists");
			Regions.CreateVariable(macroName, true);
			Regions[macroName] = new Macro();
			return null;
		}

		public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

	   public override string ToString() => $"macro {macroName}({parameterBlock.Parameters}) {{{parameterBlock.Block}}}";
	}
}
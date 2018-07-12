using Orange.Library.Managers;
using Orange.Library.Values;
using Standard.Types.Collections;

namespace Orange.Library.Verbs
{
	public class CreateGrammar : Verb
	{
		string grammarName;
		Hash<string, Pattern> patterns;
		string firstRule;

		public CreateGrammar(string grammarName, Hash<string, Pattern> patterns, string firstRule)
		{
			this.grammarName = grammarName;
			this.patterns = patterns;
			this.firstRule = firstRule;
		}

		public override Value Evaluate()
		{
			RegionManager.Regions.CreateVariable(grammarName, true);
			RegionManager.Regions[grammarName] = new Grammar(patterns, firstRule);
			return null;
		}

		public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Push;
	}
}
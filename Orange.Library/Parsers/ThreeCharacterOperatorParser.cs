using Orange.Library.Verbs;
using static System.Activator;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.TwoCharacterOperatorParser;

namespace Orange.Library.Parsers
{
	public class ThreeCharacterOperatorParser : Parser
	{
		public ThreeCharacterOperatorParser()
			: base(@"^ /(|sp|) /(['+*//%=!<>&|@#~.,\^?;:-']3)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var type = Operator(tokens[2]);
			if (type == null)
				return null;
			Color(position, tokens[1].Length, Whitespaces);
			Color(tokens[2].Length, Operators);
			var verb = (Verb)CreateInstance(type);
			verb.IsOperator = true;
			return verb;
		}

		public override string VerboseName => "three character operators";
	}
}
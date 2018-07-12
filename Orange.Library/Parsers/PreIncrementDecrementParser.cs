using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class PreIncrementDecrementParser : Parser
	{
		public PreIncrementDecrementParser()
			: base("^ /(|sp|) /('++' | '--') -(> ' '+)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
		   Color(position, tokens[1].Length, Whitespaces);
			Color(tokens[2].Length, Operators);
			return tokens[2] == "++" ? (Verb)new PreIncrement() : new PreDecrement();
		}

		public override string VerboseName => "pre-increment/decrement";
	}
}
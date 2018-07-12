using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class EndBlockParametersParser : Parser
	{
		public EndBlockParametersParser()
			: base("^ /(/s*) ['}])|']")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[0].Length, Structures);
			return new NullOp();
		}

		public override string VerboseName => "end+";

	   public override bool EndOfBlock => true;
	}
}
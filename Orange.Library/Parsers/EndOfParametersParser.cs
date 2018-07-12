using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class EndOfParametersParser : Parser
	{
		bool use;

		public EndOfParametersParser(bool use)
			: base("^ /s* ['|>)']") => this.use = use;

	   public override Verb CreateVerb(string[] tokens)
		{
			if (use)
			{
				Color(position, length, Structures);
				return new NullOp();
			}
			return null;
		}

		public override string VerboseName => "end of parameters";

	   public override bool EndOfBlock => use;
	}
}
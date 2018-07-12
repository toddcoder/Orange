using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class FormatLiteralParser : Parser
	{
		public FormatLiteralParser()
			: base("^ ' '* /'$' /(['cdefgnprxs'] '-'? (/d+)? '.'? /d*)", true)
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Formats);
			return new Push(tokens[2]);
		}

		public override string VerboseName => "format literal";
	}
}
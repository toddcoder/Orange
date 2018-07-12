using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class SingleLineCommentParser : Parser
	{
		public SingleLineCommentParser()
			: base("^ /s* '//' '//' -[/r /n]*")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Comments);
			return new NullOp();
		}

		public override string VerboseName => "single line comment";
	}
}
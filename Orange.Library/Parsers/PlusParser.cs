using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class PlusParser : Parser
	{
		public PlusParser()
			: base(@"^(\s*\+)(?=@?" + Runtime.REGEX_VARIABLE1 + "({@])")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Verbics);
			return new Plus();
		}

		public override string VerboseName
		{
			get
			{
				return "plus";
			}
		}
	}
}
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class PadderParser : Parser
	{
		public PadderParser(string pattern, bool ignoreCase = false, bool multiline = false)
			: base(pattern, ignoreCase, multiline)
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return null;
			}
		}
	}
}
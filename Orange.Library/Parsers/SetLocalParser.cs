using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class SetLocalParser : Parser
	{
		public SetLocalParser()
			: base(@"^(\s*/)(?=" + Runtime.REGEX_VARIABLE + ")")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Verb);
			return new Define();
		}

		public override string VerboseName
		{
			get
			{
				return "set local";
			}
		}
	}
}
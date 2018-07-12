using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
	public class PlaceholderParser : Parser
	{
		public PlaceholderParser()
			: base($"^ /s* '*' /({REGEX_VARIABLE})")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Variables);
			return new Push(new Placeholder(tokens[1]));
		}

		public override string VerboseName => "Placeholder";
	}
}
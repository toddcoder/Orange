using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
	public class MessageVerbParser : Parser
	{
		public MessageVerbParser()
			: base($"^ /s* /({REGEX_VARIABLE}) ':'")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, EntityType.Operators);
			return new MessageVerb(tokens[1]);
		}

		public override string VerboseName => "Message verb";
	}
}
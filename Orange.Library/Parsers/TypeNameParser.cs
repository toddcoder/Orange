using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
	public class TypeNameParser : Parser
	{
		public TypeNameParser()
			: base($"^ ' '* '#' /({REGEX_VARIABLE})")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Variables);
			result.Value = new TypeName(tokens[1]);
			return new Push(result.Value);
		}

		public override string VerboseName => "type name";
	}
}
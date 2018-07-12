using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
	public class ReferenceParser : Parser
	{
		public ReferenceParser()
			: base($"^ /(|sp|) /'&' /({REGEX_VARIABLE})")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
		   var fieldName = tokens[3];

		   Color(position, tokens[1].Length, Whitespaces);
		   Color(tokens[2].Length, Operators);
		   Color(fieldName.Length, Variables);

			return new Reference(fieldName);
		}

		public override string VerboseName => "reference";
	}
}
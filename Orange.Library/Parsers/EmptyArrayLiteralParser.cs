using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class EmptyArrayLiteralParser : Parser
	{
		public EmptyArrayLiteralParser()
			: base("^ |sp| '()'")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Arrays);
			var literal = new Array();
			result.Value = literal;
			return new PushArrayLiteral(literal);
		}

		public override string VerboseName => "empty array literal";
	}
}
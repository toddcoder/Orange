using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class ReplacementLiteralParser : Parser
	{
		ReplacementParser parser;

		public ReplacementLiteralParser()
			: base("^ /s* '&'") => parser = new ReplacementParser();

	   public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, Structures);
			var index = position + length;
			if (parser.Scan(source, index))
			{
				Result.Value = new ReplacementLiteral(parser.Replacement);
				overridePosition = parser.Result.Position;
				return new Push(Result.Value);
			}
			return null;
		}

		public override string VerboseName => "replacement literal";
	}
}
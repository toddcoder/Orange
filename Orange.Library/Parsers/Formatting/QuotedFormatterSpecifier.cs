using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Formatting
{
	public class QuotedFormatterSpecifier : Parser
	{
		public QuotedFormatterSpecifier()
			: base(@"^(\s*@)['""\\]")
		{
		}
		
		public override Verb CreateVerb(string[] tokens)
		{
			int tokens1Length = tokens[1].Length;
			Color(position, tokens1Length, IDEColor.EntityType.Verb);
			var parser = new StringParser();
			if (parser.Scan(source, position + tokens1Length))
			{
				overridePosition = parser.Result.Position;
				return new Format(parser.Result.Value);
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "quoted formatter";
			}
		}
	}
}
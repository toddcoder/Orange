using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class FormatterVerbParser : Parser
	{
		public FormatterVerbParser()
			: base(@"^(\s*\\)([cdefgnprx])?(-?\d+)?(?:\.(\d+))?", true)
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			if (tokens[0].EndsWith("\\"))
				return null;
			int tokens1Length = tokens[1].Length;
			Color(position, tokens1Length, IDEColor.EntityType.Verb);
			Color(length - tokens1Length, IDEColor.EntityType.Format);
			var formatter = new Formatter(tokens[2], tokens[3], tokens[4]);
			return new Format(formatter);
		}

		public override string VerboseName
		{
			get
			{
				return "formatter";
			}
		}
	}
}
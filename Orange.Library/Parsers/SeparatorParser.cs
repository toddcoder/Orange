using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class SeparatorParser : Parser
	{
		StringParser stringParser;
		InterpolatedStringParser interpolatedStringParser;
		public SeparatorParser()
			: base(@"^(\s*/)")
		{
			stringParser = new StringParser();
			interpolatedStringParser = new InterpolatedStringParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			int index = position + length;
			if (stringParser.Scan(source, index))
			{
				Color(position, length, IDEColor.EntityType.Verbics);
				overridePosition = stringParser.Result.Position;
				return new Push(new Separator((String)stringParser.Result.Value));
			}
			if (interpolatedStringParser.Scan(source, index))
			{
				Color(position, length, IDEColor.EntityType.Verbics);
				overridePosition = interpolatedStringParser.Result.Position;
				return new Push(new Separator((String)interpolatedStringParser.Result.Value));
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "Separator";
			}
		}
	}
}
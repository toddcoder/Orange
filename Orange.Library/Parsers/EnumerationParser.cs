using Orange.Library.Parsers.Enumerations;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class EnumerationParser : Parser
	{
		PlainEnumerationNameParser plainParser;
		NumberEnumerationNameParser numberParser;
		EndEnumerationParser endParser;

		public EnumerationParser()
			: base("^ /(/s* 'enum') /(/s* '{')")
		{
			plainParser = new PlainEnumerationNameParser();
			numberParser = new NumberEnumerationNameParser();
			endParser = new EndEnumerationParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, KeyWords);
			Color(tokens[2].Length, Structures);
			var index = position + length;
			var builder = new EnumerationBuilder();
			plainParser.Builder = builder;
			numberParser.Builder = builder;
			while (true)
			{
				if (numberParser.Scan(source, index))
				{
					index = numberParser.Result.Position;
					continue;
				}
				if (plainParser.Scan(source, index))
				{
					index = plainParser.Result.Position;
					continue;
				}
				if (endParser.Scan(source, index))
				{
					overridePosition = endParser.Result.Position;
					var enumeration = builder.Enumeration();
					return new Push(enumeration);
				}
				break;
			}
			return null;
		}

		public override string VerboseName => "enumeration parser";
	}
}
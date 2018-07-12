using System.Collections.Generic;
using System.Linq;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class ValueParser : Parser
	{
		List<Parser> parsers;

		public ValueParser()
			: base("")
		{
			parsers = new List<Parser>
			{
				new HexParser(),
				new BinParser(),
				new OctParser(),
				//new BooleanParser(),
				new RationalParser(),
				new FloatParser(),
				new IntegerParser(),
				new StringParser(),
				new InterpolatedStringParser(),
				new ArrayLiteralParser(),
				new DateParser(),
				//new ParameterParser(true),
				new SpecialValueParser(),
				new SymbolParser(),
				new PrintBlockParser(),
				new LambdaParser(),
				new BlockParser(),
				new PatternParser(),
				new FormatLiteralParser(), 
				new VariableParser()
			};
		}

		public override Verb CreateVerb(string[] tokens)
		{
			return null;
		}

		public override bool Scan(string source, int position)
		{
			foreach (var parser in parsers.Where(p => p.Scan(source, position)))
			{
				Result.Value = parser.Result.Value;
				Result.Position = parser.Result.Position;
				return true;
			}
			return false;
		}

		public override string VerboseName
		{
			get
			{
				return "values";
			}
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers.Classes
{
	public class InitializedVariableParser : Parser, IClassParser
	{
		List<Parser> parsers;
		Matcher matcher;
		MultiMethodParser multiMethodParser;

		public InitializedVariableParser()
			: base(@"^(\s*)" + Runtime.REGEX_CLASS_MEMBER + "(" + Runtime.REGEX_VARIABLE + @")(\s*=\s*)")
		{
			parsers = new List<Parser>
			{
				new HexParser(),
				new BinParser(),
				new OctParser(),
				//new BooleanParser(),
				new FloatParser(),
				new IntegerParser(),
				new StringParser(),
				new InterpolatedStringParser(),
				new ArrayLiteralParser(),
				new DateParser(),
				new ParameterParser(true),
				new SpecialValueParser(),
				new SymbolParser(),
				new BlockParser(),
				new PatternParser(),
				new FormatLiteralParser()
			};
			matcher = new Matcher();
			multiMethodParser = new MultiMethodParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespace);
			string visibility = tokens[2];
			Color(visibility.Length, IDEColor.EntityType.Verb);
			string scope = tokens[3];
			Color(scope.Length, IDEColor.EntityType.Verb);
			ClassParser.SetScopeAndVisibility(scope, visibility, this);
			string messageName = tokens[4];
			Color(messageName.Length, IDEColor.EntityType.Variable);
			Color(tokens[5].Length, IDEColor.EntityType.Verb);
			int index = position + length;
			var classParser = new ClassParser();
			parsers.Insert(0, classParser);
			foreach (Parser parser in parsers.Where(p => p.Scan(source, index)))
			{
				if (parser is ClassParser)
				{
					index = parser.Result.Position;
					if (matcher.IsMatch(source.Substring(index), @"^\.new\b"))
					{
						Color(1, IDEColor.EntityType.Structure);
						int matchedLength = matcher[0].Length;
						Color(matchedLength - 1, IDEColor.EntityType.Message);
						var builderValue = (ClassBuilderValue)parser.Result.Value;
						ClassBuilder builder = builderValue.Builder;
						builder.AutoInstantiate = true;
						index += matchedLength;
					}
					else
						index = parser.Result.Position;
				}
				else
					index = parser.Result.Position;
				Value value = parser.Result.Value;
				Block block = null;
				if (multiMethodParser.Scan(source, index))
				{
					ParserResult innerResult = multiMethodParser.Result;
					block = (Block)innerResult.Value;
					index = innerResult.Position;
				}
				Builder.AddInitializedVariable(messageName, value, this, block);
				overridePosition = index;
				return new NullOp();
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "initialized variable";
			}
		}

		public ClassBuilder Builder
		{
			get;
			set;
		}

		public bool EndOfClass
		{
			get
			{
				return false;
			}
		}

		public Class.VisibilityType Visibility
		{
			get;
			set;
		}

		public Class.ScopeType Scope
		{
			get;
			set;
		}
	}
}
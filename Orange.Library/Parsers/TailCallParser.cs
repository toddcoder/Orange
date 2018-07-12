using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class TailCallParser : Parser
	{
		Matcher matcher;
		ClosureParser closureParser;

		public TailCallParser()
			: base(@"^(\s*call)")
		{
			matcher = new Matcher();
			closureParser = new ClosureParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.KeyWord);
			var index = position + length;
			if (closureParser.Scan(source, index))
			{
				var closure = (Closure)(closureParser.Result.Value);
				index = closureParser.Result.Position;
				if (matcher.IsMatch(source.Substring(index), @"^(\s*until)(\s*\()"))
				{
					Color(index, matcher[0, 1].Length, IDEColor.EntityType.KeyWord);
					Color(matcher[0, 2].Length, IDEColor.EntityType.Structure);
					index += matcher[0].Length;
					var checkExpression = OrangeCompiler.Block(source, ref index);
					if (matcher.IsMatch(source.Substring(index), @"^(\s*then)(\s*\()"))
					{
						Color(index, matcher[0, 1].Length, IDEColor.EntityType.KeyWord);
						Color(matcher[0, 2].Length, IDEColor.EntityType.Structure);
						index += matcher[0].Length;
						var finalValue = OrangeCompiler.Block(source, ref index);
						overridePosition = index;
						return new TailCall(closure, checkExpression, finalValue);
					}
				}
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "tail call";
			}
		}
	}
}
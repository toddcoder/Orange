using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.OrangeCompiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.TwoCharacterOperatorParser;

namespace Orange.Library.Parsers
{
	public class IndexParser : Parser
	{
		public IndexParser()
			: base("^ '<' -(> /s+)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, 1, Structures);
			var index = position + 1;
			if (source.Skip(index).IsMatch(REGEX_PUNCTUATION))
				return null;
		   Block block;
			ParseBlock(source, index, "'>'").Assign(out block, out index);
			var arguments = new Arguments(block);
			overridePosition = index;

			return new Index(arguments);
		}

		public override string VerboseName => "Index";
	}
}
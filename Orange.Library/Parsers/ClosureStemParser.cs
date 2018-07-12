using Orange.Library.Values;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class ClosureStemParser
	{
		public static bool StemAhead(string source, int index)
		{
			return source.Substring(index).IsMatch(Runtime.REGEX_STEM);
		}

		Matcher matcher;
		AnyBlockParser blockParser;

		public ClosureStemParser()
		{
			matcher = new Matcher();
			blockParser = new AnyBlockParser();
		}

		public Block Parse(string source, ref int index)
		{
			if (matcher.IsMatch(source.Substring(index), Runtime.REGEX_STEM))
			{
				var length = matcher[0].Length;
				Parser.Color(index, length, IDEColor.EntityType.Structure);
				index += length;
				return blockParser.Parse(source, ref index, false);
			}
			return null;
		}
	}
}
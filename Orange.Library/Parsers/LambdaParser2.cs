using Orange.Library.Values;
using Standard.Types.RegularExpressions;

namespace Orange.Library.Parsers
{
	public class LambdaParser2
	{
		Matcher matcher;
		AnyBlockParser blockParser;

		public LambdaParser2()
		{
			matcher = new Matcher();
			blockParser = new AnyBlockParser(AnyBlockParser.REGEX_LAMBDA_BRIDGE);
		}

		public Block Parse(string source, ref int index, out bool isMacro, out bool splatting)
		{
			isMacro = false;
			var input = source.Substring(index);
			if (matcher.IsMatch(input, "^ /s* ['=-'] '>'"))
			{
				var length = matcher[0].Length;
				Parser.Color(index, length, IDEColor.EntityType.Structure);
				index += length;
				var block = blockParser.Parse(source, ref index, false);
				splatting = blockParser.Splatting;
				return block;
			}
			splatting = false;
			return null;
		}
	}
}
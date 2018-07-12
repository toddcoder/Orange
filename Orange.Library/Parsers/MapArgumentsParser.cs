using Standard.Types.RegularExpressions;

namespace Orange.Library.Parsers
{
	public class MapArgumentsParser
	{
		Matcher matcher;

		public MapArgumentsParser()
		{
			matcher = new Matcher();
		}

		public bool Parse(string source, ref int index, out Arguments arguments)
		{
			if (matcher.IsMatch(source.Substring(index), "^/s* '->' /s* '('"))
			{
				var length = matcher[0].Length;
				Parser.Color(index, length, IDEColor.EntityType.Structure);
				index += length;
				var block = OrangeCompiler.ParseBlock(source, ref index, ")");
				arguments = new Arguments(block);
				return true;
			}
			arguments = null;
			return false;
		}
	}
}
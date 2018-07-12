using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class AnyBlockParser
	{
		public const string REGEX_STANDARD_BRIDGE = "^/s* ':'";
		public const string REGEX_LAMBDA_BRIDGE = "^/s* /('->' | '=>')";

		string bridge;
		Matcher matcher;

		public AnyBlockParser(string bridge)
		{
			this.bridge = bridge;
			matcher = new Matcher();
		}

		public Block Parse(string source, ref int position, bool addEnd)
		{
			IsMacro = false;
			Splatting = false;
			if (source.Skip(position).IsMatch("^ /s* ['{(']"))
			{
				var blockParser = new BlockParser(true);
				if (blockParser.Scan(source, position))
				{
					position = blockParser.Result.Position;
					SingleLine = false;
					Splatting = false;
					return (Block)blockParser.Result.Value;
				}
				Splatting = false;
				return null;
			}
			if (source.Skip(position).IsMatch("^ /s* '.('"))
			{
				var macroParser = new MacroLiteralParser();
				if (macroParser.Scan(source, position))
				{
					position = macroParser.Result.Position;
					SingleLine = false;
					IsMacro = true;
					Splatting = false;
					return (Block)macroParser.Result.Value;
				}
			}
			SingleLine = true;
			Splatting = false;
			if (bridge.IsNotEmpty())
			{
				if (matcher.IsMatch(source.Substring(position), bridge))
				{
					var length = matcher[0].Length;
					Parser.Color(length, Structures);
					position += length;
					Splatting = matcher.GroupCount(0) > 1 && matcher[0, 1] == "=>";
				}
				else
					return null;
			}
			var oneLineBlockParser = new OneLineBlockParser(addEnd);
		   return oneLineBlockParser.Parse(source, ref position) ? oneLineBlockParser.Block : null;
		}

		public bool SingleLine
		{
			get;
			set;
		}

		public bool IsMacro
		{
			get;
			set;
		}

		public bool Splatting
		{
			get;
			set;
		}
	}
}
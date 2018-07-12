using System.Text;
using Orange.Library.Verbs;
using Standard.Types.Exceptions;
using Standard.Types.RegularExpressions;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
	public class StringParser : Parser
	{
		enum QuotePositionType
		{
			First,
			Escaped,
			Content,
			Unicode
		}

	   public StringParser()
			: base("^ |sp| /([quote])")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var text = new StringBuilder();
			var unicode = new StringBuilder();
			var quote = tokens[1][0];
			var type = QuotePositionType.First;
			var start = position;
			for (var i = position; i < source.Length; i++)
			{
				var ch = source[i];
				switch (type)
				{
					case QuotePositionType.First:
						if (ch == quote)
						{
							type = QuotePositionType.Content;
							Color(position, i - position, Whitespaces);
							start = i;
						}
						break;
					case QuotePositionType.Escaped:
						switch (ch)
						{
							case 't':
								text.Append('\t');
								break;
							case 'r':
								text.Append('\r');
								break;
							case 'n':
								text.Append('\n');
								break;
							case 'l':
								text.Append("\r\n");
								break;
							case 'u':
								type = QuotePositionType.Unicode;
								unicode.Clear();
								continue;
							default:
								text.Append("`");
								text.Append(ch);
								break;
						}
						type = QuotePositionType.Content;
						break;
					case QuotePositionType.Content:
						if (ch == quote)
						{
							overridePosition = i + 1;
							Color(start, i - start + 1, Strings);
							result.Value = Runtime.ReplaceEscapedValues(text.ToString());
							return new Push(result.Value);
						}
						if (ch == '`')
							type = QuotePositionType.Escaped;
						else
							text.Append(ch);
						break;
					case QuotePositionType.Unicode:
						if (ch.ToString().IsMatch("['0-9a-fA-F_']"))
							unicode.Append(ch);
						else
						{
							var number = HexParser.GetNumber(unicode.ToString());
							text.Append((char)number);
							type = QuotePositionType.Content;
							if (ch != '`')
								goto case QuotePositionType.Content;
						}
						break;
				}
			}
			throw "Open string".Throws();
		}

		public override string VerboseName => "string";
	}
}
using Orange.Library.Parsers.XML;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class XMLParser : Parser
	{
		const string LOC_XML_PARSER = "XML Parser";

		public static Value Parse(Parser parser, string source, ref int position)
		{
			if (parser.Scan(source, position))
			{
				position = parser.Result.Position;
				return parser.Result.Value;
			}
			return null;
		}

		public XMLParser()
			: base(@"^\s*<%(?!%)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			var index = position + length;

			Parser parser = new IndexerParser();
			if (parser.Scan(source, index))
			{
				result.Value = parser.Result.Value;
				overridePosition = parser.Result.Position;
				return parser.Result.Verb;
			}

			var name = getName(source, ref index);

			Block attributes;
			parser = new XMLAttributesParser();
			if (parser.Scan(source, index))
			{
				attributes = (Block)parser.Result.Value;
				index = parser.Result.Position;
			}
			else
				attributes = null;

			var innerText = getInnerText(source, ref index);

			var xml = new Values.XML(name, attributes, innerText);

			getChildren(source, ref index, xml);

			var matcher = new Matcher();
			Runtime.Assert(matcher.IsMatch(source.Substring(index), @"^\s*%>"), LOC_XML_PARSER, "XML not properly terminated");
			var terminatorLength = matcher[0].Length;
			Color(terminatorLength, IDEColor.EntityType.Structure);

			result.Value = xml;
			overridePosition = index + terminatorLength;
			return new Push(xml);
		}

		static Value getName(string source, ref int index)
		{
			Parser parser = new StringParser();
			var name = Parse(parser, source, ref index);
			if (name != null)
				return name;
			parser = new InterpolatedStringParser();
			name = Parse(parser, source, ref index);
			if (name != null)
				return name;
			parser = new VariableParser();
			name = Parse(parser, source, ref index);
			Runtime.RejectNull(name, LOC_XML_PARSER, "Didn't recognize {0}", source.Substring(index));
			return name;
		}

		static Value getInnerText(string source, ref int index)
		{
			Parser parser = new StringParser();
			var innerText = Parse(parser, source, ref index);
			if (innerText != null)
				return innerText;
			parser = new InterpolatedStringParser();
			innerText = Parse(parser, source, ref index);
			if (innerText != null)
				return innerText;
			parser = new BlockParser(false);
			innerText = Parse(parser, source, ref index);
			return innerText;
		}

		static void getChildren(string source, ref int index, Values.XML xml)
		{
			Parser parser = new XMLParser();
			while (parser.Scan(source, index))
			{
				xml.AddChild((Values.XML)parser.Result.Value);
				index = parser.Result.Position;
			}

			var matcher = new Matcher();
			if (matcher.IsMatch(source.Substring(index), @"^\s*<%%"))
			{
				var openLength = matcher[0].Length;
				Color(index, openLength, IDEColor.EntityType.Structure);
				index += openLength;
				xml.Children = OrangeCompiler.Block(source, ref index, "%>");
			}

			parser = new IndirectXMLParser();
			while (parser.Scan(source, index))
			{
				xml.AddChild((Values.XML)parser.Result.Value);
				index = parser.Result.Position;
			}
		}

		public override string VerboseName
		{
			get
			{
				return "xml";
			}
		}
	}
}
using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Parsers.Templates;
using Orange.Library.Templates;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using Print = Orange.Library.Orange.Library.Templates.Print;

namespace Orange.Library.Parsers
{
	public class TemplateParser2 : Parser
	{
		List<Parser> parsers;

		public TemplateParser2()
			: base(@"^\s*<\$")
		{
			parsers = new List<Parser>
			{
				new CodeTemplateParser(),
				new PutTemplateParser(),
				new WriteTemplateParser(),
				new BlankLineTemplateParser(),
				new PadderBeginTemplateParser(),
				new PadderEndTemplateParser(),
				new PadderEvaluateTemplateParser(),
				new XMLBeginTemplateParser(),
				new XMLEndTemplateParser(),
				new XMLTextTemplateParser()
			};
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.String);
			string[] lines = source.Substring(position + length).Split(@"(\r?\n)");
			var matcher = new Matcher();
			var start = lines[0].HasContent() ? 0 : 2;
			var exit = false;
			int newLength = length + (start == 0 ? 0 : lines[0].Length + lines[1].Length);
			var items = new List<Item>();
			int linesLength = lines.Length;
			for (int i = start; i < linesLength && !exit; i += 2)
			{
				bool lineHasContent = lines[i].HasContent();
				if (lineHasContent && matcher.IsMatch(lines[i], @"^(.*)\$>(.*)$"))
				{
					string match = matcher[0, 1];
					if (match.HasContent())
					{
						Item item = getItem(match);
						items.Add(item);
					}
					newLength += match.Length + 2;
					exit = true;
				}
				else
				{
					Item item = getItem(lines[i]);
					items.Add(item);
					newLength += lines[i].Length + (i < linesLength ? lines[i + 1].Length : 0);
				}
			}
			if (items.Count > 0)
			{
				MessageManager.State.RegisterMessageCall("drop");
				MessageManager.State.RegisterMessageCall("sprint");
				Color(newLength, IDEColor.EntityType.String);
				var template = new Template(items.ToArray());
				result.Value = template;
				overridePosition = position + newLength;
				return new Push(template);
			}
			return null;
		}

		Item getItem(string text)
		{
			foreach (Parser parser in parsers)
				if (parser.Scan(text, 0))
				{
					Item item = ((ITemplateItem)parser).Item;
					item.RegisterMessages();
					return item;
				}
			if (text.StartsWith("`"))
				text = text.Substring(1);
			return new Print(text);
		}

		public override string VerboseName
		{
			get
			{
				return null;
			}
		}
	}
}
using System.Collections.Generic;
using System.Text;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class TemplateParser : Parser
	{
		enum ParsingState
		{
			Ignore,
			Text,
			Code,
			PossibleOpenBracket,
			PossibleCloseBracket,
			PossibleEnd/*,
			IgnoreLineEndingsText,
			IgnoreLineEndingsCode,
			SwitchToText,
			SwitchToCode*/
		}

		static string trim(string text)
		{
			text = text.Substitute(@"^\r\n", "");
			text = text.Substitute(@"\r\n$", "");
			text = text.Substitute(@"^\n", "");
			text = text.Substitute(@"\n$", "");
			return text;
		}

		public TemplateParser()
			: base(@"^\s*<\$")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			int index = position + length;
			var text = new StringBuilder();
			var escaped = false;
			var state = ParsingState.Ignore;
			var items = new List<Template.Item>();
			for (var i = index; i < source.Length; i++)
			{
				char chr = source[i];
				Template.Item item;
				switch (state)
				{
					case ParsingState.Ignore:
						if (chr != '\r' && chr != '\n')
						{
							state = ParsingState.Text;
							goto case ParsingState.Text;
						}
						break;
					case ParsingState.Text:
						switch (chr)
						{
							case '`':
								if (escaped)
								{
									text.Append("`");
									escaped = false;
								}
								else
									escaped = true;
								break;
							case '<':
								if (escaped)
								{
									text.Append("<");
									escaped = false;
								}
								else
									state = ParsingState.PossibleOpenBracket;
								break;
							case '$':
								if (escaped)
								{
									text.Append("$");
									escaped = false;
								}
								else
									state = ParsingState.PossibleEnd;
								break;
							default:
								text.Append(chr);
								escaped = false;
								break;
						}
						break;
					case ParsingState.Code:
						switch (chr)
						{
							case '`':
								if (escaped)
								{
									text.Append("`");
									escaped = false;
								}
								else
									escaped = true;
								break;
							case '&':
								if (escaped)
								{
									text.Append("&");
									escaped = false;
								}
								else
									state = ParsingState.PossibleCloseBracket;
								break;
							default:
								text.Append(chr);
								escaped = false;
								break;
						}
						break;
					case ParsingState.PossibleOpenBracket:
						if (escaped)
						{
							text.Append(chr);
							escaped = false;
						}
						else
							switch (chr)
							{
								case '&':
									string str = trim(text.ToString());
									if (str.Length > 0)
									{
										item = new Template.TextItem(str);
										items.Add(item);
										text.Clear();
									}
									state = ParsingState.Code;
									break;
								default:
									text.Append('<');
									text.Append(chr);
									state = ParsingState.Text;
									break;
							}
						break;
					case ParsingState.PossibleCloseBracket:
						if (escaped)
						{
							text.Append(chr);
							escaped = false;
						}
						else
							switch (chr)
							{
								case '>':
									string str = trim(text.ToString());
									if (str.Length > 0)
									{
										item = new Template.CodeItem(str);
										items.Add(item);
										text.Clear();
									}
									state = ParsingState.Text;
									break;
								default:
									text.Append('&');
									text.Append(chr);
									state = ParsingState.Code;
									break;
							}
						break;
					case ParsingState.PossibleEnd:
						if (escaped)
						{
							text.Append(chr);
							escaped = false;
							state = ParsingState.Text;
						}
						else
							switch (chr)
							{
								case '>':
									string str = trim(text.ToString());
									if (str.Length > 0)
									{
										item = new Template.TextItem(str);
										items.Add(item);
									}
									var template = new Template(items.ToArray());
									Color(i - index - 1, IDEColor.EntityType.String);
									Color(2, IDEColor.EntityType.Structure);
									result.Value = template;
									overridePosition = i + 1;
									return new PushValue(template);
								default:
									text.Append('$');
									text.Append(chr);
									state = ParsingState.Text;
									break;
							}
						break;
					/*					case ParsingState.IgnoreLineEndingsText:
											if (chr == '\r' || chr == '\n')
												state = ParsingState.SwitchToText;
											break;
										case ParsingState.SwitchToText:
											if (chr != '\r' && chr != '\n')
											{
												state = ParsingState.Text;
												goto case ParsingState.Text;
											}
											break;
										case ParsingState.IgnoreLineEndingsCode:
											if (chr == '\r' || chr == '\n')
												state = ParsingState.SwitchToCode;
											break;
										case ParsingState.SwitchToCode:
											if (chr != '\r' && chr != '\n')
											{
												state = ParsingState.Code;
												goto case ParsingState.Code;
											}
											break;*/
				}
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "template";
			}
		}
	}
}
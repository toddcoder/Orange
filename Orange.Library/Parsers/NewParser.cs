using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class NewParser : Parser
	{
		public NewParser()
			: base(@"^(\s*)(" + Runtime.REGEX_VARIABLE + @")(\s*{)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespaces);
			var className = tokens[2];
			Color(className.Length, IDEColor.EntityType.Variables);
			Color(tokens[3].Length, IDEColor.EntityType.Structures);
			var index = position + length;
			var block = OrangeCompiler.ParseBlock(source, ref index, "}");
			overridePosition = index;
			return new SendNewMessage(className, block);
		}

		public override string VerboseName
		{
			get
			{
				return "new";
			}
		}
	}
}
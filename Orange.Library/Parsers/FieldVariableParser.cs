using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
	public class FieldVariableParser : Parser
	{
		public FieldVariableParser()
			: base(@"^(\s*\$)(\d+|\()")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			var tokens1Length = tokens[1].Length;
			Color(position, tokens1Length, IDEColor.EntityType.Variable);
			Value value;
			if (tokens[2] == "(")
			{
				Color(1, IDEColor.EntityType.Structure);
				var index = position + tokens1Length + 1;
				var block = OrangeCompiler.Block(source, ref index, ")", true);
				overridePosition = index;
				value = new FieldBlockVariable(block);
			}
			else
			{
				Color(tokens[2].Length, IDEColor.EntityType.Number);
				var index = tokens[2].ToInt();
				value = new FieldVariable(index);
			}
			result.Value = value;
			return new Push(value);
		}

		public override string VerboseName
		{
			get
			{
				return "field variable";
			}
		}
	}
}
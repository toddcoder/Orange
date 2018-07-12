using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class SpecialAssignmentsParser : Parser
	{
		public SpecialAssignmentsParser()
			: base(@"^(\s*)(abstract|todo|has)(\s+)(" + Runtime.REGEX_VARIABLE + ")")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			string keyWord = tokens[2];
			string variableName = tokens[4];
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespace);
			Color(keyWord.Length, IDEColor.EntityType.KeyWord);
			Color(tokens[3].Length, IDEColor.EntityType.Whitespace);
			Color(variableName.Length, IDEColor.EntityType.Variable);
			switch (keyWord)
			{
				case "abstract":
					return new SpecialAssignment(variableName, new Abstract());
				case "todo":
					return new SpecialAssignment(variableName, new ToDo());
				case "has":
					return new SpecialAssignment("has", variableName);
				default:
					return null;
			}
		}

		public override string VerboseName
		{
			get
			{
				return "special assignment";
			}
		}
	}
}
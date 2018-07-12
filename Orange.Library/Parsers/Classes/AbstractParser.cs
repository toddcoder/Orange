using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class AbstractParser : Parser, IClassParser
	{
		public AbstractParser()
			: base(@"^(\s+)" + Runtime.REGEX_CLASS_MEMBER + "(" + Runtime.REGEX_VARIABLE + @")(\s*->\?)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespace);
			string visibility = tokens[2];
			Color(visibility.Length, IDEColor.EntityType.Verb);
			string scope = tokens[3];
			ClassParser.SetScopeAndVisibility(scope, visibility, this);
			Color(scope.Length, IDEColor.EntityType.Verb);
			string messageName = tokens[4];
			Color(messageName.Length, IDEColor.EntityType.Variable);
			Color(tokens[5].Length, IDEColor.EntityType.Verb);
			Builder.AddAbstractMessage(messageName, this);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "Abstract";
			}
		}

		public ClassBuilder Builder
		{
			get;
			set;
		}

		public bool EndOfClass
		{
			get
			{
				return false;
			}
		}

		public Class.VisibilityType Visibility
		{
			get;
			set;
		}

		public Class.ScopeType Scope
		{
			get;
			set;
		}
	}
}
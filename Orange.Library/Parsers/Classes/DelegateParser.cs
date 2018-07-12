using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class DelegateParser : Parser, IClassParser
	{
		public DelegateParser()
			: base(@"^(\s+)" + Runtime.REGEX_CLASS_MEMBER + "(" + Runtime.REGEX_VARIABLE + @")(\s*->\s*)(super\.)?(" + Runtime.REGEX_VARIABLE +
			")")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespace);
			string visibility = tokens[2];
			Color(visibility.Length,IDEColor.EntityType.Verb);
			string scope = tokens[3];
			Color(scope.Length, IDEColor.EntityType.Verb);
			ClassParser.SetScopeAndVisibility(scope, visibility, this);
			string message = tokens[4];
			Color(message.Length, IDEColor.EntityType.Variable);
			Color(tokens[5].Length, IDEColor.EntityType.Verb);
			string super = tokens[6];
			int superLength = super.Length;
			if (superLength > 0)
			{
				Color(superLength - 1, IDEColor.EntityType.Variable);
				Color(1, IDEColor.EntityType.Verb);
			}
			string messageDelegatedTo = tokens[7];
			Color(messageDelegatedTo.Length, IDEColor.EntityType.Variable);
			Builder.AddDelegate(message, super + messageDelegatedTo, this);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "Delegate";
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
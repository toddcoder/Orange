using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class ChangeVisibilityOrScopeParser : Parser, IClassParser
	{
		public ChangeVisibilityOrScopeParser()
			: base(@"^\s*([()[\]|])")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			switch (tokens[1])
			{
				case "(":
					Builder.PushVisibility(Class.VisibilityType.Private);
					break;
				case "[":
					Builder.PushVisibility(Class.VisibilityType.Protected);
					break;
				case ")":
				case "]":
					Builder.PopVisibility();
					break;
				case "|":
					Builder.ToggleScope();
					break;
			}
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "change visibility or scope";
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
using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class ChangeStateParser : Parser, IClassParser
	{
		public ChangeStateParser()
			: base(@"^(\s*\.)(pub|priv|prot|inst|class)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Structure);
			string change = tokens[2];
			Color(change.Length, IDEColor.EntityType.EndAction);
			switch (change.ToLower())
			{
				case "pub":
					Builder.SetVisibility(Class.VisibilityType.Public);
					break;
				case "priv":
					Builder.SetVisibility(Class.VisibilityType.Private);
					break;
				case "prot":
					Builder.SetVisibility(Class.VisibilityType.Protected);
					break;
				case "inst":
					Builder.SetScope(Class.ScopeType.Instance);
					break;
				case "class":
					Builder.SetScope(Class.ScopeType.Class);
					break;
				default:
					return null;
			}
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "change state";
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
	}
}
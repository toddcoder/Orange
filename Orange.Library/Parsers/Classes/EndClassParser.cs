using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class EndClassParser : Parser, IClassParser
	{
		public EndClassParser()
			: base(@"^\s*}")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "end class";
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
				return true;
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
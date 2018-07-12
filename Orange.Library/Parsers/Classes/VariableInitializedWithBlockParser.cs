using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class VariableInitializedWithBlockParser : Parser, IClassParser
	{
		public VariableInitializedWithBlockParser()
			: base(@"^(\s*)" + Runtime.REGEX_CLASS_MEMBER + "(" + Runtime.REGEX_VARIABLE + @")(\s*=\s*)(\()(?!\|)")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Whitespace);
			string visibility = tokens[2];
			Color(visibility.Length, IDEColor.EntityType.Verb);
			string scope = tokens[3];
			Color(scope.Length, IDEColor.EntityType.Verb);
			ClassParser.SetScopeAndVisibility(scope, visibility, this);
			string messageName = tokens[4];
			Color(messageName.Length, IDEColor.EntityType.Variable);
			Color(tokens[5].Length, IDEColor.EntityType.Verb);
			Color(tokens[6].Length, IDEColor.EntityType.Structure);

			int index = position + length;
			var compiler = new OrangeCompiler(source, index);
			Block block = compiler.Compile();
			result.Position = compiler.Position;
			Builder.AddInitializedVariable(messageName, block, this, null);
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "variable initialized with a block";
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
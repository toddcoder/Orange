using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class MethodParser : Parser, IClassParser
	{
		ClosureParser closureParser;
		ParameterParser parameterParser;

		public MethodParser()
			: base(@"^(\s*)" + Runtime.REGEX_CLASS_MEMBER + "(" + Runtime.REGEX_VARIABLE + @")(\s*=)")
		{
			closureParser = new ClosureParser();
			parameterParser = new ParameterParser(true);
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
			
			if (closureParser.Scan(source, position + length))
			{
				var closure = (Closure)closureParser.Result.Value;
				int parserPosition = closureParser.Result.Position;
				var multiMethodParser = new MultiMethodParser();
				if (multiMethodParser.Scan(source, parserPosition))
				{
					var block = (Block)multiMethodParser.Result.Value;
					Builder.AddMultiMethod(messageName, closure, block, this);
					overridePosition = multiMethodParser.Result.Position;
				}
				else
				{
					Builder.AddMethod(messageName, closure, this);
					overridePosition = parserPosition;
				}
				return new NullOp();
			}
			if (parameterParser.Scan(source, position + length))
			{
				var closure = parameterParser.Result.Value as Closure;
				if (closure == null)
					return null;
				var multiMethodParser = new MultiMethodParser();
				int parserPosition = parameterParser.Result.Position;
				if (multiMethodParser.Scan(source, parserPosition))
				{
					var block = (Block)multiMethodParser.Result.Value;
					Builder.AddMultiMethod(messageName, closure, block, this);
					overridePosition = multiMethodParser.Result.Position;
				}
				else
				{
					Builder.AddMethod(messageName, closure, this);
					overridePosition = parserPosition;
				}
				return new NullOp();
			}
			return null;
		}

		public override string VerboseName
		{
			get
			{
				return "method";
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
using System.Collections.Generic;
using Orange.Library.Classes;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class SingleLineMethodParser : Parser, IClassParser
	{
		FunctionBodyParser functionBodyParser;
		EqualBlockParser equalBlockParser;
		MultiMethodParser multiMethodParser;

		public SingleLineMethodParser()
			: base(@"^(\s*)" + Runtime.REGEX_CLASS_MEMBER + "(" + Runtime.REGEX_VARIABLE + @")(\()")
		{
			functionBodyParser = new FunctionBodyParser();
			equalBlockParser = new EqualBlockParser();
			multiMethodParser = new MultiMethodParser();
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
			Color(tokens[5].Length, IDEColor.EntityType.Structure);

			var compiler = new OrangeCompiler(source, position + length);
			Block parameterBlock = compiler.Compile();
			int index = compiler.Position;
			List<Parameter> parameterList = ParameterParser.GetParameterList(parameterBlock);
			var parameters = new Parameters(parameterList);
			if (functionBodyParser.Scan(source, index))
				return createMethod(messageName, functionBodyParser, parameters, false);
			return equalBlockParser.Scan(source, index) ? createMethod(messageName, equalBlockParser, parameters, true) : null;
		}

		Verb createMethod(string messageName, Parser parser, Parameters parameters, bool includeMultiMethod)
		{
			var block = (Block)parser.Result.Value;
			int index = parser.Result.Position;
			var closure = new Closure(new Namespace(), block, parameters, false);
			if (includeMultiMethod && multiMethodParser.Scan(source, index))
			{
				var comparisand = (Block)multiMethodParser.Result.Value;
				Builder.AddMultiMethod(messageName, closure, comparisand, this);
				index = multiMethodParser.Result.Position;
			}
			else
				Builder.AddMethod(messageName, closure, this);
			overridePosition = index;
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "single line method";
			}
		}

		public ClassBuilder Builder
		{
			get;
			set;
		}

		public bool EndOfClass
		{
			get;
			private set;
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
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class ArrayFieldsParameterParser : Parser
	{
		BlockParser parser;

		public ArrayFieldsParameterParser()
			: base(@"^(\s*->)(\s*\|:)")
		{
			parser = new BlockParser();
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position,tokens[1].Length,IDEColor.EntityType.Verb);
			Color(tokens[2].Length, IDEColor.EntityType.Structure);
			var index = position + length;
			var block = OrangeCompiler.Block(source, ref index, true);
			var parameterList = ParameterParser.GetParameterList(block);
			var parameters = new Parameters(new[]
			{
				new Parameter(""),
				new Parameter(""),
				new Parameter("")
			});
			if (parser.Scan(source, index))
			{
				block = (Block)parser.Result.Value;
				index = parser.Result.Position;
			}
			else
				block = null;
			overridePosition = index;
			var unpackedVariables = parameterList.Select(t => t.Name).ToList();
			return new PushArrayParameters(parameters, block, unpackedVariables);
		}

		public override string VerboseName
		{
			get
			{
				return "Array field parameters";
			}
		}
	}
}
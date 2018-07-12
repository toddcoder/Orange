using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Classes
{
	public class MultiMethodParser : Parser
	{
		public MultiMethodParser()
			: base(@"^(\s*\?)(\s*\()")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, tokens[1].Length, IDEColor.EntityType.Verb);
			Color(tokens[2].Length, IDEColor.EntityType.Structure);
			var compiler = new OrangeCompiler(source, position + length);
			Block block = compiler.Compile();
			result.Value = block;
			overridePosition = compiler.Position;
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "multimethod";
			}
		}
	}
}
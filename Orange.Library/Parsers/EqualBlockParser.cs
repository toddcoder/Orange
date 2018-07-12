using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class EqualBlockParser : Parser
	{
		public EqualBlockParser()
			: base(@"^\s*=\s*{")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structure);
			var index = position + length;
			var block = OrangeCompiler.ParseBlock(source, ref index, "}");
			result.Value = block;
			overridePosition = index;
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "equal block";
			}
		}
	}
}
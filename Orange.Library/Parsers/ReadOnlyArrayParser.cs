using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public class ReadOnlyArrayParser : Parser
	{
		public ReadOnlyArrayParser()
			: base(@"^\s*\^\(")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position, length, IDEColor.EntityType.Structures);
			var index = position + length;
			var block = OrangeCompiler.ParseBlock(source, ref index, ")");
			overridePosition = index;
			return new CreateReadOnlyArray(block);
		}

		public override string VerboseName
		{
			get
			{
				return "Read-only array";
			}
		}
	}
}
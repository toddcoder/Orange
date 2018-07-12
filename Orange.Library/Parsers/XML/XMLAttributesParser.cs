using Orange.Library.Verbs;

namespace Orange.Library.Parsers.XML
{
	public class XMLAttributesParser : Parser
	{
		public XMLAttributesParser()
			: base(@"^\s*\(")
		{
		}

		public override Verb CreateVerb(string[] tokens)
		{
			Color(position,length,IDEColor.EntityType.Structure);
			var index = position + length;
			var block = OrangeCompiler.Block(source, ref index, ")");
			result.Value = block;
			overridePosition = index;
			return new NullOp();
		}

		public override string VerboseName
		{
			get
			{
				return "xml attributes";
			}
		}
	}
}
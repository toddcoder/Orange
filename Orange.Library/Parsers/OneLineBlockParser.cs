using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
	public static class OneLineBlockParser
	{
		public static Block Parse(string source, ref int position, bool addEnd)
		{
			var block = OrangeCompiler.Block(source, ref position, Runtime.REGEX_END, OrangeCompiler.ConsumeEndOfBlockType.SingleLine);
			if (addEnd)
				block.Add(new End());
			return block;
		}
	}
}
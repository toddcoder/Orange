using System.IO;

namespace Orange.Library.Templates
{
	public class PadderEnd : Item
	{
		public PadderEnd()
			: base("")
		{
		}

		public override void Render(StringWriter writer, string variableName)
		{
			writer.WriteLine("{0}.print('{1}');", Runtime.VAR_PADDER, variableName);
		}
	}
}
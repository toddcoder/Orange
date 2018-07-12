using System.IO;

namespace Orange.Library.Templates
{
	public class PadderBegin : Item
	{
		public PadderBegin(string text)
			: base(text)
		{
		}

		public PadderBegin()
			: this("")
		{
		}

		public override void Render(StringWriter writer, string variableName)
		{
			writer.WriteLine("{0} = ({1}).padder;", Runtime.VAR_PADDER, text);
		}
	}
}
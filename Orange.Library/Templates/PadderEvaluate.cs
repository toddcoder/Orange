using System.IO;

namespace Orange.Library.Templates
{
	public class PadderEvaluate : Item
	{
		public PadderEvaluate(string text)
			: base(text)
		{
		}

		public PadderEvaluate()
			: this("")
		{
		}

		public override void Render(StringWriter writer, string variableName)
		{
			writer.WriteLine("({0}) ? {1};", text, Runtime.VAR_PADDER);
		}
	}
}
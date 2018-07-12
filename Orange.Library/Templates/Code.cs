using System.IO;

namespace Orange.Library.Templates
{
	public class Code : Item
	{
		public Code(string text)
			: base(text)
		{
		}

		public Code()
			: this("")
		{
		}

		public override void Render(StringWriter writer, string variableName)
		{
			writer.WriteLine(text);
		}
	}
}
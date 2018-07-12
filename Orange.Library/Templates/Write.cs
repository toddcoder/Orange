using System.IO;
using Standard.Types.Strings;

namespace Orange.Library.Templates
{
	public class Write : Item
	{
		public Write(string text)
			: base(text)
		{
		}

		public Write()
			: base("")
		{
		}

		public override void Render(StringWriter writer, string variableName)
		{
			if (text.IsNotEmpty())
				writer.WriteLine($"$write = $<{text}>;");
		}
	}
}
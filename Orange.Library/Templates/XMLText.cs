using System.IO;
using Orange.Library.Managers;

namespace Orange.Library.Templates
{
	public class XMLText : Item
	{
		public XMLText(string text)
			: base(text)
		{
		}

		public XMLText()
			: base("")
		{
		}

		public override void Render(StringWriter writer, string variableName)
		{
			writer.WriteLine("|{0}|.xmlify.write('{1}');", text, variableName);
		}

		public override void RegisterMessages()
		{
			MessageManager.MessagingState.RegisterMessageCall("xmlify");
			MessageManager.MessagingState.RegisterMessageCall("write");
		}
	}
}
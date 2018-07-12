using System.IO;
using Orange.Library.Managers;
using Standard.Types.RegularExpressions;

namespace Orange.Library.Templates
{
	public abstract class Item
	{
		protected string text;

		public Item(string text) => this.text = text;

	   public abstract void Render(StringWriter writer, string variableName);

		public virtual void RegisterMessages()
		{
			var matcher = new Matcher();
			matcher.Evaluate(text, Runtime.REGEX_SEND_MESSAGE, true);
			for (var i = 0; i < matcher.MatchCount; i++)
				MessageManager.MessagingState.RegisterMessageCall(matcher[i, 3]);
		}
	}
}
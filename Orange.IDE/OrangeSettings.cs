using Standard.Applications;
using Standard.Computer;

namespace Orange.IDE
{
	public class OrangeSettings : Settings<OrangeSettings>
	{
		public FolderName CurrentFolder
		{
			get;
			set;
		}

		public FileName LastSourceFile
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public override void Initialize()
		{
			CurrentFolder = @"C:\Enterprise\Orange";
			LastSourceFile = null;
			Text = "";
		}
	}
}
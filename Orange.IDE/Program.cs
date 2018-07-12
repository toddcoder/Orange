using System;
using System.Windows.Forms;
using Standard.Applications;
using Standard.Computer;

namespace Orange.IDE
{
	static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			FileName fileName = null;
			var arguments = new Arguments(args);
		   var aFileName = arguments[0].FileName;
			if (aFileName.IsSome)
				fileName = aFileName.Value;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new OrangeIDE
			{
				CodeFile = fileName
			});
		 }
	}
}
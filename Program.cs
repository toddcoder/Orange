using System;
using Orange.Library;
using Standard.Applications;
using Standard.Computer;
using static Orange.Library.NewOrangeCompiler;
using Arguments = Standard.Applications.Arguments;

namespace Orange
{
	class Program : CommandLine
	{
		static void Main(string[] args)
		{
			var program = new Program
			{
				Test = true
			};
			program.Run(args);
		}

		public override void Execute(Arguments arguments)
		{
			arguments.AssertCount(1);
			FileName sourceFile = arguments[0].Text;
			var source = sourceFile.Text;
			var block = Compile(source);
			var runtime = new OrangeRuntime(block);
			var text = runtime.Execute();
			Console.WriteLine(text);
		}

		string ask(string prompt) => prompt;
	}
}

using System;
using System.IO;
using Orange.Library.Managers;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.NewOrangeCompiler;
using static Orange.Library.Parsers.Parser;

namespace Orange.Library
{
   public class Orange
   {
      string source;
      IColorizer colorizer;
      string verboseText;
      IFileCache fileCache;
      Block block;
      IConsole console;

      public Orange(string source, IColorizer colorizer = null, IFileCache fileCache = null, IConsole console = null)
      {
         this.source = source.Trim();
         this.colorizer = colorizer;
         this.fileCache = fileCache;
         this.console = console;
         verboseText = "";
         block = null;
      }

      public Func<string> Ask { get; set; }

      public string VerboseText => verboseText;

      public bool AutoVariable { get; set; }

      public string Text { get; set; } = "";

      public string[] ModuleFolders { get; set; } = new string[0];


      public string Execute()
      {
         CompilerState.Reset();
         IDEColors.Clear();
         Coloring = true;
         MessagingState = new MessageManager();
         block = Compile(source);
         verboseText = "";
         Coloring = false;
         if (AutoVariable)
         {
            block.Add(new End());
            block.Add(new Push(new Variable("$out")));
            block.Add(new Assign());
            block.Add(new Push(new Variable("sys")));
            block.Add(new SendMessage("dump", new Arguments()));
            block.Add(new End());
         }
         var runtime = new OrangeRuntime(block, Text, fileCache, console) { ModuleFolders = ModuleFolders };
         try
         {
            var result = runtime.Execute();
            LastValue = runtime.LastValue;
            LastType = runtime.LastType;
            return result;
         }
         catch (Stop.StopException exception)
         {
            LastValue = exception.Message;
            LastType = "Stopped";
            return $"Stopped: {exception.Message}";
         }
      }

      public string LastValue { get; set; } = "";

      public string LastType { get; set; } = "";

      public string ColorizeOnly()
      {
         IDEColors.Clear();
         Coloring = true;
         MessagingState = new MessageManager();
         Compile(source);
         /*			var compiler = new OrangeCompiler(source)
                  {
                     Verbose = verboseCompile
                  };
                  compiler.Compile();*/
         verboseText = ""; //compiler.VerboseText;
         Coloring = false;
         return "";
      }

      public void Colorize() => colorizer?.Colorize();

      public Block Block => block;

      public static string Dump() => "";

      public string DumpAll()
      {
         using (var writer = new StringWriter())
         {
            writer.WriteLine(Region.LINE_DIVIDER);
            writer.WriteLine(Region.LINE_COUNT0);
            writer.WriteLine(Region.LINE_COUNT1);
            writer.WriteLine(Regions.Current.Dump());
            writer.WriteLine(Dump());
            return writer.ToString();
         }
      }
   }
}
using Standard.Applications;
using static System.Console;
using Arguments = Standard.Applications.Arguments;

namespace Orange.REPL
{
   internal class Program : CommandLine
   {
      Code code;

      static void Main(string[] args)
      {
         var program = new Program();
         program.Run(args);
      }

      public override void Execute(Arguments arguments)
      {
         Test = arguments.Count > 0 && arguments[0].Text == "test";

         code = new Code();
         var running = true;

         while (running)
         {
            Write($"orange{code.Prompt}> ");
            var line = ReadLine();
            switch (line)
            {
               case "#quit":
               case "#exit":
                  running = false;
                  if (Test)
                     WriteLine("Hit enter to quit");
                  continue;
               case "#show":
                  WriteLine(code);
                  break;
               case "#reset":
                  code.Reset();
                  Clear();
                  break;
               case "#redo":
                  code.Execute().If(WriteLine).Else(e => WriteLine(e.Message));
                  break;
               case "#cls":
                  Clear();
                  break;
               case "#list":
                  WriteLine(code.List());
                  break;
               default:
                  if (line.StartsWith("#edit "))
                  {
                     code.Edit(line).If(WriteLine).Else(e => WriteLine(e.Message));
                     continue;
                  }

                  if (line.StartsWith("#del"))
                  {
                     code.Delete(line).If(WriteLine).Else(e => WriteLine(e.Message));
                     continue;
                  }

                  if (line.StartsWith("#"))
                  {
                     WriteLine($"Didn't understand command {line}");
                     continue;
                  }

                  code.AddLine(line);
                  break;
            }
         }
      }
   }
}
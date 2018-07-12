using System;
using Standard.Computer;
using static System.Windows.Forms.Application;
using static Standard.Types.Maybe.MaybeFunctions;

namespace OrangePlayground
{
   internal static class Program
   {
      [STAThread]
      static void Main(string[] args)
      {
         var passedFile = when(args.Length > 0, () => (FileName)args[0]);
         EnableVisualStyles();
         SetCompatibleTextRenderingDefault(false);
         Run(new Playground { PassedFileName = passedFile });
      }
   }
}
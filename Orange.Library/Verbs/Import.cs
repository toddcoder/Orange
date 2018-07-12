using System.Linq;
using Orange.Library.Values;
using Standard.Computer;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.NewOrangeCompiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Import : Verb, IStatement
   {
      const string LOCATION = "Import";

      string path;
      string result;

      public Import(string path)
      {
         this.path = path;
         result = "";
      }

      public override Value Evaluate()
      {
         var source = getSource();
         var block = Compile(source);
         result = $"{path} imported";
         Block.Registering = false;
         block.Evaluate(Regions.Global);
         Block.Registering = true;
         return null;
      }

      string getSource()
      {
         if (path.Contains("\\"))
            return ((FileName)path).Text;

         var fullName = path.EndsWith(".orange") ? path : path + ".orange";

         foreach (var fileName in State.ModuleFolders.Select(folder => folder + fullName).Where(fileName => fileName.Exists()))
            return fileName.Text;

         Throw(LOCATION, $"Couldn't find module {path}");
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}
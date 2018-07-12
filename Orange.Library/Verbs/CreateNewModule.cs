using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class CreateNewModule : Verb
   {
      string moduleName;
      NewModule module;

      public CreateNewModule(string moduleName, NewModule module)
      {
         this.moduleName = moduleName;
         this.module = module;
      }

      public override Value Evaluate()
      {
         var result = module.Evaluate();
         Regions.Current.CreateAndSet(moduleName, module);
         return result;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"module {moduleName} {module}";
   }
}
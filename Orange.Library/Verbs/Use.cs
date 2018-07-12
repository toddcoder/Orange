using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Use : Verb
   {
      const string LOCATION = "Use";

      string moduleName;

      public Use(string moduleName) => this.moduleName = moduleName;

      public override Value Evaluate()
      {
         var value = Regions[moduleName];
         if (value is Object obj)
         {
            var region = obj.Region.Public();
            foreach (var item in region.AllVariables())
            {
               if (!Regions.VariableExists(item.Key))
                  Regions.CreateVariable(item.Key);
               Regions[item.Key] = item.Value;
            }
         }
         else
            Throw(LOCATION, $"{value} isn't an object");

         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => "use";
   }
}
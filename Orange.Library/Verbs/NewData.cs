using Orange.Library.Values;
using Standard.Types.Collections;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class NewData : Verb
   {
      Data data;

      public NewData(string name, Hash<string, int> constructors) => data = new Data(name, constructors);

      public override Value Evaluate()
      {
         var current = Regions.Current;
         current.CreateAndSet(data.Name, data);
         foreach (var item in data.Constructors)
         {
            Value value;
            if (item.Value == 0)
               value = new Constructor(data.Name, item.Key, new Value[0]);
            else
               value = new ConstructorProxy(data.Name, item.Key);
            if (!current.ContainsMessage(item.Key))
               current.CreateAndSet(item.Key, value);
         }

         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => data.ToString();
   }
}
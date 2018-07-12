using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library
{
   public class WithRegion : Region
   {
      const string LOCATION = "With region";

      Object obj;

      public WithRegion(Object obj) => this.obj = obj;

      public override Value this[string name]
      {
         get
         {
            if (ContainsMessage(name))
               return SendMessage(obj, name);
            return null;
         }
         set
         {
            if (ContainsMessage(name))
               SendMessage(obj, name, value);
         }
      }

      public override bool ContainsMessage(string messageName) => obj.RespondsNoDefault(messageName) && obj.IsPublic(messageName);
   }
}
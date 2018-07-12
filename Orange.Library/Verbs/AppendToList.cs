using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class AppendToList : Verb
   {
      const string LOCATION = "Append to List";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         InternalList list;
         if (stack.IsEmpty)
         {
            if (y.Type == ValueType.Nil)
               return y;

            list = new InternalList();
            list.Add(y);
            return list;
         }

         var x = stack.Pop(true, LOCATION);
         if (y.Type == ValueType.Nil)
            return x;

         list = x as InternalList;
         if (list != null)
         {
            list.Add(y);
            return list;
         }

         list = new InternalList();
         list.Add(x);
         list.Add(y);
         return list;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.CreateArray;

      public override string ToString() => "::";
   }
}
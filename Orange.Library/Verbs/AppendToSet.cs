using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class AppendToSet : Verb
   {
      const string LOCATION = "Append to set";

      public override Value Evaluate()
      {
         var stack = Runtime.State.Stack;
         var y = stack.Pop(true, LOCATION);
         if (stack.IsEmpty)
         {
            var set = new Set(new[] { y });
            return set;
         }

         var x = stack.Pop(true, LOCATION);
         if (x is Set xSet)
         {
            xSet.Add(y);
            return xSet;
         }

         xSet = new Set(new[] { x, y });
         return xSet;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.CreateArray;

      public override string ToString() => "^^";
   }
}
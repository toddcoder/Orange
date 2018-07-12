using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Verbs.NSRange;

namespace Orange.Library.Verbs
{
   public class NSCountedRange : Verb
   {
      const string LOCATION = "Counted range";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var right = stack.Pop(true, LOCATION);
         var left = stack.Pop(true, LOCATION);
         if (right.Int < 0)
         {
            left = left.Int + right.Int + 1;
            right = left.Int - right.Int - 1;
         }
         else
            right = left.Int + right.Int - 1;
         return GetGenerator(left, right, true);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

      public override string ToString() => ".:";
   }
}
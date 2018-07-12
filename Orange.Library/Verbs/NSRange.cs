using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class NSRange : Verb
   {
      protected const string LOCATION = "Range";

      public static Value GetGenerator(Value left, Value right, bool inclusive)
      {
         if (left.Type == ValueType.Number && right.Type == ValueType.Number)
            return new NSIntRange(left.Int, right.Int, inclusive);
         if (left.Type == ValueType.String && right.Type == ValueType.String)
            return new NSStringRange(left.Text, right.Text, inclusive);
         if (left.Type == ValueType.Date && right.Type == ValueType.Date)
            return new NSDateRange((Date)left, (Date)right, inclusive);
         if (left.Type == ValueType.Object && right.Type == ValueType.Object)
            return new NSObjectRange((Object)left, (Object)right);

         Throw(LOCATION, "Not a range");
         return null;
      }

      protected bool inclusive;

      public NSRange() => inclusive = true;

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var right = stack.Pop(true, LOCATION);
         var left = stack.Pop(true, LOCATION);
         return GetGenerator(left.Self, right.Self, inclusive);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

      public override string ToString() => "..";
   }
}
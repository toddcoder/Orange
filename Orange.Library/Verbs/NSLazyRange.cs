using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class NSLazyRange : Verb
   {
      const string LOCATION = "Range";

      public static Value GetGenerator(Value seed, Value increment)
      {
         if (increment.Type == ValueType.Lambda)
            return new Values.NSLazyRange(seed, (Lambda)increment);

         Throw(LOCATION, "Not a range");
         return null;
      }

      protected bool inclusive;

      public NSLazyRange() => inclusive = true;

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var increment = stack.Pop(true, LOCATION);
         var seed = stack.Pop(true, LOCATION);
         return GetGenerator(seed.Self, increment.Self);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

      public override string ToString() => ".*";
   }
}
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class NSBy : Verb
   {
      const string LOCATION = "By";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var right = increment(stack);
         var left = stack.Pop(true, LOCATION);
         if (left is NSIntRange intRange && right.Type == ValueType.Number)
            return new NSIntRange(intRange, right.Int);

         Throw(LOCATION, "Value isn't a range");
         return null;
      }

      protected virtual Value increment(ValueStack stack) => stack.Pop(true, LOCATION);

      public override VerbPresidenceType Presidence => VerbPresidenceType.Range;

      public override string ToString() => ".+";
   }
}
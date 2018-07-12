using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class LessThanEqual : Verb
   {
      const string LOCATION = "Less than equal";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(true, LOCATION);
         if (x.IsArray && y.IsArray)
         {
            var verb = new PlainLessThanEqual(x, y);
            return verb.Evaluate();
         }

         if (x.Type == ValueType.Set && y.Type == ValueType.Set)
            return SendMessage(x, "isPropSubset", y);

         switch (x.Type)
         {
            case ValueType.Ternary:
               var ternary = (Ternary)x;
               return ternary.IsTrue && ternary.Value.Compare(y) <= 0;
            default:
               ternary = new Ternary
               {
                  Truth = Compare(x, y) <= 0
               };
               if (ternary.IsTrue)
                  ternary.Value = y.Resolve();
               return ternary;
         }
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.LessThanEqual;

      public override string ToString() => "<=";
   }
}
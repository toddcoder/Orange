using Orange.Library.Managers;
using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class TernaryOp : Verb
   {
      const string STR_LOCATION = "Ternary operation";

      public override Value Evaluate()
      {
         var value = Runtime.State.Stack.Pop(false, STR_LOCATION);
         var special = Runtime.State.Stack.Pop(true, STR_LOCATION);
         Ternary ternary;
         switch (special.Type)
         {
            case Value.ValueType.Ternary:
               ternary = (Ternary)special;
               return ternary.IsTrue && ternary.Value.Compare(value) <= 0;
            default:
               ternary = new Ternary
               {
                  Truth = value.Compare(special) >= 0
               };
               if (ternary.IsTrue)
                  ternary.Value = value.Resolve();
               return ternary;
         }
      }

      public override ExpressionManager.VerbPresidenceType Presidence => ExpressionManager.VerbPresidenceType.Apply;

      public override string ToString() => ":=:";
   }
}
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class DefaultTo : TwoValueVerb
   {
      public override Value Evaluate(Value x, Value y)
      {
         if (x is Some some)
            return some.Value();
         if (x is None)
            return y;

         return x.IsEmpty ? y : x;
      }

      public override string Location => "Default to";

      public override string Message => "defaultTo";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Or;

      public override string ToString() => "??";
   }
}
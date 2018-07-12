using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class DivBy : TwoValueVerb
   {
      public override VerbPresidenceType Presidence => VerbPresidenceType.Divide;

      public override Value Evaluate(Value x, Value y)
      {
         var divisor = y.Number;
         Reject(divisor == 0, Location, "Divide by 0");
         return x.Number % divisor == 0;
      }

      public override string Location => "Divide by?";

      public override string Message => "isDiv";

      public override string ToString() => "/?";
   }
}
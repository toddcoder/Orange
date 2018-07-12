using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class GreaterThanEqual : ComparisonVerb
   {
      public override string ToString() => ">=";

      public override bool Compare(int comparison) => comparison >= 0;

      public override VerbPresidenceType Presidence => VerbPresidenceType.GreaterThanEqual;

      public override string Location => "Greater than equal";

      public override Value Exception(Value x, Value y)
      {
         if (x.Type == ValueType.Set && y.Type == ValueType.Set)
            return SendMessage(x, "isPropSuperset", y);
         return null;
      }
   }
}
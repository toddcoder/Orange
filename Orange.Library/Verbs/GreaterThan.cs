using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class GreaterThan : ComparisonVerb
   {
      public override bool Compare(int comparison) => comparison > 0;

      public override string ToString() => ">";

      public override VerbPresidenceType Presidence => VerbPresidenceType.GreaterThan;

      public override string Location => "Greater than";

      public override Value Exception(Value x, Value y)
      {
         if (x is VerbBinding verbBinding)
            return verbBinding.Evaluate(y);
         if (x.Type == ValueType.Set && y.Type == ValueType.Set)
            return SendMessage(x, "isSuperset", y);

         return null;
      }
   }
}
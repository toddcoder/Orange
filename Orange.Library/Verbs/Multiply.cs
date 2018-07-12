using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Multiply : TwoValueVerb
   {
      public override Value Evaluate(Value x, Value y) => x.Number * y.Number;

      public override Value Exception(Value x, Value y)
      {
         switch (x.Type)
         {
            case Value.ValueType.String:
               if (y.IsNumeric())
                  return SendMessage(x, "repeat", y);

               break;
         }
         switch (y.Type)
         {
            case Value.ValueType.String:
               if (x.IsNumeric())
                  return SendMessage(y, "repeat", x);

               break;
         }

         return null;
      }

      public override string Location => "Multiply";

      public override string Message => "mult";

      public override string ToString() => "*";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Multiply;
   }
}
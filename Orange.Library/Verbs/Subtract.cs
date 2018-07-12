using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Subtract : TwoValueVerb
   {
      public override Value Evaluate(Value x, Value y) => x.Number - y.Number;

      public override Value Exception(Value x, Value y)
      {
         /*         if (x.ProvidesGenerator)
                     return SendMessage(x, y.IsNumeric() ? "skip" : "skipUntil", Arguments.FromValue(y));*/
         if (x.IsArray && y.Type == Value.ValueType.String)
            return SendMessage(x, Message, y);

         return null;
      }

      public override string Location => "Subtract";

      public override string Message => "sub";

      public override string ToString() => "-";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Subtract;
   }
}
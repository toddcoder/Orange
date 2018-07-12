using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class Add : TwoValueVerb
   {
      public override Value Evaluate(Value x, Value y) => x.Number + y.Number;

      public override Value Exception(Value x, Value y)
      {
         if (x.Type == ValueType.Object && y.Type == ValueType.Trait)
         {
            var obj = (Object)x;
            var trait = (Trait)y;
            obj.Stack(trait);
            return obj;
         }

         if (x.IsArray && y.Type == ValueType.KeyedValue)
            return SendMessage(x, Message, y);
         if (x is List && y.Type != ValueType.List)
            return SendMessage(x, Message, y);
         if (x.Type == ValueType.String && !x.IsNumeric() && y.Type == ValueType.String && !y.IsNumeric())
            return x.Text + y.Text;

         return null;
      }

      public override string Location => "Add";

      public override string Message => "add";

      public override string ToString() => "+";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Add;
   }
}
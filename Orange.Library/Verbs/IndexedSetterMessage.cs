using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class IndexedSetterMessage : IndexedSetter
   {
      protected string message;

      public IndexedSetterMessage(string fieldName, string message, Block index, IMatched<Verb> verb, Block expression,
         bool insert)
         : base(fieldName, index, verb, expression, insert) => this.message = message;

      protected override Value getValue()
      {
         var value = base.getValue();
         return SendMessage(value, message);
      }

      public override string ToString()
      {
         return $"{fieldName}.{message}[{(insert ? "+" : "")}{index}] {verb.FlatMap(v => v.ToString(), () => "")}= {expression}";
      }
   }
}
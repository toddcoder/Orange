using Orange.Library.Managers;
using Standard.Types.Collections;

namespace Orange.Library.Values
{
   public class Grammar : Value
   {
      // ReSharper disable once NotAccessedField.Local
      Hash<string, Pattern> patterns;
      // ReSharper disable once NotAccessedField.Local
      string firstRule;

      public Grammar(Hash<string, Pattern> patterns, string firstRule)
      {
         this.patterns = patterns;
         this.firstRule = firstRule;
      }

      public Grammar() => patterns = new Hash<string, Pattern>();

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Grammar;

      public override bool IsTrue => false;

      public override Value Clone() => null;

      protected override void registerMessages(MessageManager manager) { }

      public Value Apply() => null;
   }
}
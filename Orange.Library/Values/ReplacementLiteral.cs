using Orange.Library.Managers;
using Orange.Library.Replacements;

namespace Orange.Library.Values
{
   public class ReplacementLiteral : Value
   {
      IReplacement replacement;

      public ReplacementLiteral(IReplacement replacement) => this.replacement = replacement;

      public IReplacement Replacement => replacement;

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.ReplacementLiteral;

      public override bool IsTrue => false;

      public override Value Clone() => new ReplacementLiteral(replacement.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((ReplacementLiteral)v).Apply());
      }

      public Value Apply()
      {
         var value = Arguments.ApplyValue;
         if (value is Pattern pattern)
         {
            pattern = (Pattern)pattern.Clone();
            pattern.Replacement = replacement;
            return pattern;
         }

         return value;
      }
   }
}
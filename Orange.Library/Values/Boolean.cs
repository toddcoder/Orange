using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Boolean : Value
   {
      bool booleanValue;

      public Boolean(bool booleanValue) => this.booleanValue = booleanValue;

      public Boolean() => booleanValue = false;

      public override string ToString() => booleanValue ? "true" : "false";

      public override int Compare(Value value) => booleanValue.CompareTo(value.IsTrue);

      public override string Text
      {
         get => booleanValue ? "true" : "false";
         set => booleanValue = value == "true";
      }

      public override double Number
      {
         get => booleanValue ? 1 : 0;
         set => booleanValue = (int)value != 0;
      }

      public override ValueType Type => ValueType.Boolean;

      public override bool IsTrue => booleanValue;

      public override Value Clone() => new Boolean(booleanValue);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "not", v => !v.IsTrue);
      }

      public override Value AlternateValue(string message) => new Double(booleanValue ? 1 : 0);

      public Value Else()
      {
         if (Arguments.Executable.CanExecute && booleanValue)
            Arguments.Executable.Evaluate();
         return null;
      }

      public Value ElseIf()
      {
         if (!booleanValue)
            return false;
         if (Arguments.Executable.CanExecute && Arguments[0].IsTrue)
         {
            Arguments.Executable.Evaluate();
            return false;
         }
         return true;
      }

      public Block Block => CodeBuilder.PushValue(this);
   }
}
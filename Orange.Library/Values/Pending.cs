using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Pending : Value
   {
      const string LOCATION = "Pending";

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            throwException();
            return null;
         }
         set { }
      }

      static void throwException()
      {
         Runtime.Throw(LOCATION, "Variable not initialized");
      }

      public override double Number
      {
         get
         {
            throwException();
            return 0;
         }
         set { }
      }

      public override ValueType Type => ValueType.Pending;

      public override bool IsTrue => false;

      public override Value Clone() => null;

      protected override void registerMessages(MessageManager manager) { }

      public override string ToString() => "pending";
   }
}
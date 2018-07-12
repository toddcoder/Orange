using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class Ignore : Value
   {
      public static Ignore IgnoreValue => new Ignore();

      public override int Compare(Value value) => value.Type == ValueType.Ignore ? 0 : -1;

      public override string Text
      {
         get;
         set;
      } = "";

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.Ignore;

      public override bool IsTrue => false;

      public override Value Clone() => new Ignore();

      protected override void registerMessages(MessageManager manager)
      {
      }

      public override string ToString() => "ignore";

      public override bool IsIgnore => true;
   }
}
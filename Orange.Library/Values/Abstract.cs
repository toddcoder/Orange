using Orange.Library.Managers;
using Standard.Types.Exceptions;

namespace Orange.Library.Values
{
   public class Abstract : Value
   {
      Signature signature;

      public Abstract(Signature signature) => this.signature = signature;

      public Signature Signature => signature;

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Abstract;

      public override bool IsTrue => false;

      public override Value Clone() => new Abstract((Signature)signature.Clone());

      protected override void registerMessages(MessageManager manager) => manager.RegisterMessage(this, "invoke", v => Invoke());

      public static Value Invoke() => throw "Can't invoke an abstract".Throws();

      public override string ToString() => Signature.ToString();
   }
}
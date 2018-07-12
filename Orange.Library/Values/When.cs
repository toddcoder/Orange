using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class When : Value
   {
      Block condition;
      Block result;
      Block otherwise;

      public When(Value condition, Value result)
      {
         this.condition = Block.GuaranteeBlock(condition);
         this.result = Block.GuaranteeBlock(result);
         otherwise = null;
      }

      public ParameterBlock Condition => new ParameterBlock(condition);

      public ParameterBlock Result => new ParameterBlock(result);

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get;
         set;
      }

      public override double Number
      {
         get;
         set;
      }

      public override ValueType Type => ValueType.When;

      public override bool IsTrue => condition.Evaluate().IsTrue;

      public override Value Clone() => new When(condition.Clone(), result.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((When)v).Invoke());
         manager.RegisterMessage(this, "apply", v => ((When)v).Apply());
      }

      public Value Invoke() => condition.Evaluate().IsTrue ? result.Evaluate() : new Nil();

      public Value Apply()
      {
         var value = Arguments.ApplyValue;
         return value.Type != ValueType.Nil ? value : Invoke();
      }

      public Value Otherwise
      {
         get => otherwise;
         set => otherwise = Block.GuaranteeBlock(value);
      }

      public override string ToString() => "{" + condition + "} =? {" + result + "}";
   }
}
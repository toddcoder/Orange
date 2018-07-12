using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class VerbBinding : Value
   {
      Value boundValue;
      Lambda lambda;

      public VerbBinding(Value boundValue, Lambda lambda)
      {
         this.boundValue = boundValue;
         this.lambda = lambda;
      }

      public override int Compare(Value value) => Evaluate().Compare(value);

      public override string Text
      {
         get => Evaluate().Text;
         set { }
      }

      public override double Number
      {
         get => Evaluate().Number;
         set { }
      }

      public override ValueType Type => ValueType.VerbBinding;

      public override bool IsTrue => Evaluate().IsTrue;

      public override Value Clone() => new VerbBinding(boundValue.Clone(), (Lambda)lambda.Clone());

      protected override void registerMessages(MessageManager manager) { }

      public Value Evaluate() => lambda.Invoke(new Arguments(boundValue));

      public Value Evaluate(Value secondValue)
      {
         var arguments = new Arguments(new[] { boundValue, secondValue });
         return lambda.Invoke(arguments);
      }

      public override string ToString() => $"{boundValue} < {lambda}";
   }
}
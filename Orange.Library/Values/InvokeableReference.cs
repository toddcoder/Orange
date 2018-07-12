using Orange.Library.Managers;
using Standard.Types.Maybe;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class InvokeableReference : Value
   {
      const string LOCATION = "Invokeable reference";

      string variableName;

      public InvokeableReference(string variableName) => this.variableName = variableName;

      public override int Compare(Value value) => value is Signature s ? (MatchesSignature(s) ? 0 : 1) : -1;

      public bool MatchesSignature(Signature signature)
      {
         var invokeable = State.GetInvokeable(variableName);
         if (invokeable == null)
            return false;

         return variableName.EndsWith(signature.Name) && invokeable.Matches(signature);
      }

      public override string Text
      {
         get
         {
            var invokeable = State.GetInvokeable(variableName);
            if (invokeable == null)
               return "";

            return invokeable.ImmediatelyInvokeable ? invokeable.Invoke(new Arguments()).Text : "";
         }
         set { }
      }

      public override double Number
      {
         get
         {
            var invokeable = State.GetInvokeable(variableName);
            if (invokeable == null)
               return 0;

            return invokeable.ImmediatelyInvokeable ? invokeable.Invoke(new Arguments()).Number : 0;
         }
         set { }
      }

      public override ValueType Type => ValueType.InvokeableReference;

      public override bool IsTrue => false;

      public override Value Clone() => new InvokeableReference(variableName);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((InvokeableReference)v).Invoke());
      }

      public Value Invoke(Arguments arguments)
      {
         var invokeable = State.GetInvokeable(variableName);
         RejectNull(invokeable, LOCATION, $"Invokeable for {variableName} not found");
         invokeable.ObjectRegion = ObjectRegion;
         var value = invokeable.Invoke(arguments);
         return value;
      }

      public Value Invoke() => Invoke(Arguments);

      public string VariableName => variableName;

      public Region Region { get; set; }

      public IInvokeable Invokeable
      {
         get => State.GetInvokeable(variableName);
         set => State.SetInvokeable(variableName, value);
      }

      public override Value AssignmentValue() => (Value)Invokeable;

      public override Value AlternateValue(string message) => (Value)Invokeable;

      public IMaybe<ObjectRegion> ObjectRegion { get; set; } = none<ObjectRegion>();
   }
}
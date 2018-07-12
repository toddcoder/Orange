using Orange.Library.Managers;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class Delegate : Value, IInvokeable
   {
      const string LOCATION = "Delegate";

      string target;
      string targetMessage;

      public Delegate(string target, string targetMessage)
      {
         this.target = target;
         this.targetMessage = targetMessage;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Delegate;

      public override bool IsTrue => false;

      public override Value Clone() => new Delegate(target, targetMessage);

      protected override void registerMessages(MessageManager manager) { }

      public Value Invoke(Arguments arguments)
      {
         if (target.IsEmpty())
         {
            var value = RegionManager.Regions[targetMessage];
            if (value is IInvokeable invokeable)
               return invokeable.Invoke(arguments);

            Throw(LOCATION, $"{targetMessage} isn't an invokeable");
            return null;
         }

         var targetObject = RegionManager.Regions[target];
         return SendMessage(targetObject, targetMessage, arguments);
      }

      public Region Region { get; set; }

      public bool ImmediatelyInvokeable { get; set; }

      public int ParameterCount => 0;

      public bool Matches(Signature signature)
      {
         if (target.IsEmpty())
         {
            if (RegionManager.Regions.VariableExists("self"))
               if (RegionManager.Regions["self"] is Object obj)
               {
                  if (obj.Region[targetMessage] is InvokeableReference invokeableReference)
                     return invokeableReference.MatchesSignature(signature);
               }
               else
                  Throw(LOCATION, "Self not an object");

            if (RegionManager.Regions[targetMessage] is InvokeableReference reference)
               return reference.MatchesSignature(signature);

            Throw(LOCATION, $"{targetMessage} isn't invokeable");
         }

         if (RegionManager.Regions[target] is IInvokeable invokeable)
            return invokeable.Matches(signature);

         Throw(LOCATION, $"{targetMessage} isn't an invokeable");
         return false;
      }

      public bool ReturnNull { get; set; }

      public bool Initializer { get; set; }

      public IMaybe<ObjectRegion> ObjectRegion { get; set; } = none<ObjectRegion>();
   }
}
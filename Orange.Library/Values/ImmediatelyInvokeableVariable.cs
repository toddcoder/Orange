using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class ImmediatelyInvokeableVariable : Variable
   {
      public ImmediatelyInvokeableVariable(string name)
         : base(name) { }

      public ImmediatelyInvokeableVariable() { }

      public override Value Value
      {
         get
         {
            var value = RegionManager.Regions[Name];
            if (value is IImmediatelyInvokeable invokeable && invokeable.ImmediatelyInvokeable)
               return Runtime.SendMessage(value, "invoke");

            return value;
         }
         set => base.Value = value;
      }
   }
}
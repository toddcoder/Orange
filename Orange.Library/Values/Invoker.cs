using Orange.Library.Invocations;
using Orange.Library.Managers;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Invoker : Value
   {
      const string LOCATION = "Invoker";

      Invocation invocation;

      public Invoker(Invocation invocation) => this.invocation = invocation;

      public Invoker() => invocation = null;

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Invoker;

      public override bool IsTrue => false;

      public override Value Clone() => new Invoker(invocation);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((Invoker)v).Invoke());
      }

      public Value Invoke()
      {
         object obj;
         switch (invocation.Type)
         {
            case Invocation.InvocationType.Instance:
               var arguments = Arguments.Clone();
               var target = arguments.Shift();
               if (target is DotNet targetObj)
                  obj = invocation.Invoke(targetObj.Object, arguments.Values);
               else
               {
                  Throw(LOCATION, "Didn't return .NET object");
                  return null;
               }

               break;
            default:
               obj = invocation.Invoke(null, Arguments.Values);
               break;
         }

         return new DotNet(obj);
      }
   }
}
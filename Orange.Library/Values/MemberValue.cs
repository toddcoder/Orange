using System.Reflection;
using Orange.Library.Invocations;
using Orange.Library.Managers;
using Standard.Types.Numbers;

namespace Orange.Library.Values
{
   public class MemberValue : Value
   {
      object instance;
      string member;
      Bits32<BindingFlags> bindingFlags;

      public MemberValue(object instance, string member)
      {
         this.instance = instance;
         this.member = member;
         bindingFlags = BindingFlags.InvokeMethod;
         bindingFlags[BindingFlags.Public] = true;
         bindingFlags[BindingFlags.Instance] = true;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return member; }
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Member;

      public override bool IsTrue => true;

      public override Value Clone() => new MemberValue(instance, member);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((MemberValue)v).Invoke());
      }

      public Value Invoke()
      {
         var arguments = Arguments.Values.GetArguments();
         return instance.GetType().InvokeMember(member, bindingFlags, null, instance, arguments).GetValue();
      }
   }
}
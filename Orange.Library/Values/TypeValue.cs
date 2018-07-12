using System;
using System.Reflection;
using Orange.Library.Invocations;
using Orange.Library.Managers;
using Orange.Library.Messages;
using Standard.Types.Numbers;

namespace Orange.Library.Values
{
   public class TypeValue : Value, IMessageHandler
   {
      Assembly assembly;
      Type type;

      public TypeValue(Assembly assembly, string typeName)
      {
         this.assembly = assembly;
         type = assembly.GetType(typeName, false, true);
      }

      public TypeValue(Assembly assembly, Type type)
      {
         this.assembly = assembly;
         this.type = type;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => type.FullName;
         set { }
      }

      public override double Number
      {
         get { return 0; }

         set { }
      }

      public override ValueType Type => ValueType.Type;

      public override bool IsTrue => true;

      public override Value Clone() => new TypeValue(assembly, type);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "new", v => ((TypeValue)v).New());
         manager.RegisterMessage(this, "invoke", v => ((TypeValue)v).New());
      }

      public Value New()
      {
         var arguments = Arguments.Values.GetArguments();
         return new DotNet(Activator.CreateInstance(type, arguments));
      }

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         var memberInfos = type.GetMember(messageName);
         foreach (var info in memberInfos)
            switch (info.MemberType)
            {
               case MemberTypes.Field:
                  handled = true;
                  return new MemberVariable(messageName, null, true, type);
               case MemberTypes.Property:
                  handled = true;
                  return new MemberVariable(messageName, null, false, type);
               case MemberTypes.Method:
                  handled = true;
                  var args = arguments.Values.GetArguments();
                  Bits32<BindingFlags> bindingFlags = BindingFlags.InvokeMethod;
                  bindingFlags[BindingFlags.Public] = true;
                  bindingFlags[BindingFlags.Static] = true;
                  return type.InvokeMember(messageName, bindingFlags, null, null, args).GetValue();
            }

         handled = false;
         return null;
      }

      public bool RespondsTo(string messageName) => type.GetMember(messageName).Length > 0;
   }
}
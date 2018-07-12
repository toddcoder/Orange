using System;
using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class NativeFunction : Lambda
   {
      string name;
      Func<Value[], Value> func;

      public NativeFunction(string name, Func<Value[], Value> func)
         : base(new Region(), new Block(), new Parameters(), false)
      {
         this.name = name;
         this.func = func;
      }

      public override int Compare(Value value) => 0;

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

      public override ValueType Type => ValueType.Lambda;

      public override bool IsTrue => true;

      public override Value Clone() => new NativeFunction(name, func);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((NativeFunction)v).Invoke());
      }

      public Value Invoke() => func(Arguments.Values);

      public override Value Invoke(Arguments arguments) => func(arguments.Values);

      public override string ToString() => $"{name}()";
   }
}
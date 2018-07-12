using System.Collections.Generic;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class FunctionApplication : Value
   {
      List<Lambda> functions;

      public FunctionApplication(Lambda left, Lambda right) => functions = new List<Lambda> { left, right };

      public FunctionApplication(List<Lambda> functions) => this.functions = functions;

      public void Add(Lambda lambda) => functions.Add(lambda);

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.FunctionApplication;

      public override bool IsTrue => false;

      public override Value Clone() => new FunctionApplication(functions);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((FunctionApplication)v).Invoke());
      }

      public Value Invoke()
      {
         var newArguments = Arguments.Clone();
         Value result = NilValue;
         for (var i = functions.Count - 1; i > -1; i--)
         {
            result = functions[i].Evaluate(newArguments);
            newArguments = new Arguments(result);
         }
         return result;
      }

      public override string ToString() => functions.Listify(" . ");
   }
}
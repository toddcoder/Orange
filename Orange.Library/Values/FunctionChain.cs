using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class FunctionChain : Value
   {
      const string LOCATION = "Function chain";

      List<Lambda> functions;

      public FunctionChain() => functions = new List<Lambda>();

      public FunctionChain(IEnumerable<Lambda> lambdas)
         : this() => functions.AddRange(lambdas);

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; }

      public override ValueType Type => ValueType.FunctionChain;

      public override bool IsTrue => functions.Count > 0;

      public override Value Clone() => new FunctionChain(functions.Select(f => (Lambda)f.Clone()));

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "shr", v => ((FunctionChain)v).ShiftRight());
         manager.RegisterMessage(this, "invoke", v => ((FunctionChain)v).Invoke());
      }

      public void Add(Lambda lambda) => functions.Add(lambda);

      public Value ShiftRight()
      {
         if (Arguments[0] is Lambda lambda)
            functions.Add(lambda);
         else
            Throw(LOCATION, "Right hand value must be a lambda");
         return this;
      }

      public Value Invoke()
      {
         var result = functions[0].Invoke(Arguments);
         for (var i = 1; i < functions.Count; i++)
         {
            var arguments = new Arguments(result);
            result = functions[i].Invoke(arguments);
         }

         return result;
      }

      public override string ToString() => functions.Listify(" >> ");
   }
}
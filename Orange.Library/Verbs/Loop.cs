using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Standard.Types.Lambdas.LambdaFunctions;

namespace Orange.Library.Verbs
{
   public class Loop : Verb, IStatement
   {
      Block initialization;
      bool isWhile;
      Block condition;
      Block increment;
      Block body;
      string result;

      public Loop(Block initialization, bool isWhile, Block condition, Block increment, Block body)
      {
         this.initialization = initialization;
         this.initialization.AutoRegister = false;
         this.initialization.Expression = false;

         this.isWhile = isWhile;

         this.condition = condition;
         this.condition.AutoRegister = false;
         this.condition.Expression = false;

         this.increment = increment;
         this.increment.AutoRegister = false;
         this.increment.Expression = false;

         this.body = body;
         this.body.AutoRegister = false;

         result = "";
      }

      public override Value Evaluate()
      {
         var continuing = isWhile ? func(() => condition.IsTrue) : func(() => !condition.IsTrue);
         var index = 0;
         using (var popper = new RegionPopper(new Region(), "c-for"))
         {
            popper.Push();

            for (initialization.Evaluate(); continuing() && index++ < MAX_LOOP; increment.Evaluate())
               using (var innerPopper = new RegionPopper(new Region(), "c-for-body"))
               {
                  innerPopper.Push();
                  body.Evaluate();
                  var signal = Signal();
                  if (signal == Breaking || signal == ReturningNull)
                     break;
               }

            result = index == 1 ? "1 loop" : $"{index} loops";
            return index;
         }
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}
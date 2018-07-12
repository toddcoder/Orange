using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class CaseExecute : Verb
   {
      Block comparisand;
      Block result;
      bool required;
      Block condition;

      public CaseExecute(Block comparisand, Block result, bool required, Block condition)
      {
         this.comparisand = comparisand;
         this.comparisand.AutoRegister = false;

         this.result = result;
         this.result.AutoRegister = false;

         this.required = required;

         this.condition = condition;
      }

      public override Value Evaluate()
      {
         var left = State.Stack.Pop(true, "Case");
         if (left.IsNil)
            return left;
         var right = comparisand.Evaluate();
         Case _case;
         if (left is Case c)
            _case = new Case(c, right, false, condition);
         else
            _case = new Case(left, right, false, required, condition);
         var arguments = GuaranteedExecutable(result);
         return SendMessage(_case, "then", arguments);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => $"{(required ? "required" : "case")} {comparisand}: {result}";
   }
}
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class ShiftL : Verb
   {
      public override Value Evaluate()
      {
         var stack = State.Stack;
         var source = stack.Pop(true, location(), false);
         var target = stack.Pop(false, location());
         if (target is Variable variable)
         {
            var value = variable.Value;
            var result = SendMessage(value, messageName(), source);
            variable.Value = result;
            return variable;
         }

         return SendMessage(target, messageName(), source);
      }

      protected virtual string location() => "SHL";

      protected virtual string messageName() => "shl";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "<<";
   }
}
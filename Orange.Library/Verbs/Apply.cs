using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Value;

namespace Orange.Library.Verbs
{
   public class Apply : Verb
   {
      const string LOCATION = "Apply";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var target = stack.Pop(true, LOCATION, false);
         var subject = stack.Pop(false, LOCATION);
         switch (target.Type)
         {
            case ValueType.Object:
            case ValueType.Class:
            case ValueType.Tuple:
            case ValueType.MessagePath:
               return SendMessage(target, "apply", subject);
         }

         var arguments = new Arguments();
         if (subject.IsVariable)
         {
            var variable = (Variable)subject;
            arguments.ApplyVariable = variable;
            arguments.ApplyValue = variable.Value;
         }
         else
         {
            arguments.ApplyVariable = null;
            arguments.ApplyValue = subject;
         }
         return MessagingState.SendMessage(target, "apply", arguments);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "|";
   }
}
using Orange.Library.Values;
using static Orange.Library.Arguments;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Zip : Verb
   {
      const string LOCATION = "Zip";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var value = stack.Pop(true, LOCATION);
         var target = stack.Pop(true, LOCATION);
         var arguments = FromValue(value, false);
         return SendMessage(target, "zip", arguments);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "zip";
   }
}
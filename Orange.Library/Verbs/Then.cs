using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Then : Verb
   {
      const string LOCATION = "Then";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var nextValue = stack.Pop(true, LOCATION, false);
         if (nextValue is IExecutable executable)
         {
            var seed = stack.Pop(true, LOCATION);
            return new UnboundedGenerator(seed, executable.Action);
         }

         Throw(LOCATION, "Must be an executable");
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "then";
   }
}
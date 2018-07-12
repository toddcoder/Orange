using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class EndOfDataAssign : Verb
   {
      const string LOCATION = "End of data assign";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var value = stack.Pop(true, LOCATION);
         var pop = stack.Pop(false, LOCATION);
         if (pop is Variable variable)
         {
            if (value.Type == Value.ValueType.Nil)
               return false;

            value.AssignmentValue().AssignTo(variable);
            return true;
         }

         Throw(LOCATION, "Expected a variable");
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => ":=:";
   }
}
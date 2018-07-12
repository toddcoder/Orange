using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Bind : Verb
   {
      const string LOCATION = "Bind";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var value = stack.Pop(true, LOCATION);
         var variable = stack.Pop<Variable>(false, LOCATION);
         value = value.AssignmentValue();
         value = new BoundValue(variable.Name, value);
         return value;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.KeyedValue;

      public override string ToString() => "::";
   }
}
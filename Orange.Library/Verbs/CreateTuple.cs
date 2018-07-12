using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class CreateTuple : Verb
   {
      const string LOCATION = "Create tuple";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var right = stack.Pop(true, LOCATION);
         var left = stack.Pop(true, LOCATION);
         return left is OTuple tuple ? new OTuple(tuple, right) : new OTuple(left, right);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.CreateTuple;

      public override string ToString() => ";";
   }
}
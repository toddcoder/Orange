using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Generate : Verb
   {
      const string LOCATION = "Generate";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         var x = stack.Pop(false, LOCATION);
         return x is Variable v ? (y is Block b ? new Generator(v.Name, b) : y) : x;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;
   }
}
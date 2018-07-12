using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Scan : Verb
   {
      const string LOCATION = "Scan";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, LOCATION);
         if (!(y is Lambda lambda))
         {
            Throw(LOCATION, "Lambda required as right-hand argument");
            return null;
         }

         var x = stack.Pop(true, LOCATION);
         if (!(x is INSGeneratorSource source))
         {
            Throw(LOCATION, "Left hand side must provide generator source");
            return null;
         }

         return new Scanner(source, lambda);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "<>";
   }
}
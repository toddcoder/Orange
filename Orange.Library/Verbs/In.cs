using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class In : Verb
   {
      const string LOCATION = "In";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var searchObject = stack.Pop(true, LOCATION);
         var searcher = stack.Pop(true, LOCATION);
         return SendMessage(searchObject, "in", searcher);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Equals;

      public override string ToString() => "in";
   }
}
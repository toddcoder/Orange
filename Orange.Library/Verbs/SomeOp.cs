using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class SomeOp : Verb
   {
      public override Value Evaluate()
      {
         var value = State.Stack.Pop(true, "Some op");
         return new Some(value.Self);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Increment;

      public override string ToString() => "?";
   }
}
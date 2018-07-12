using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class PushSome : Verb
   {
      Block expression;

      public PushSome(Block expression) => this.expression = expression;

      public override Value Evaluate()
      {
         var value = expression.Evaluate();
         return new Some(value);
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => $"$[{expression}]";

      public Block Expression => expression;
   }
}
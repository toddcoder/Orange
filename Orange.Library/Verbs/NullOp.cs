using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class NullOp : Verb
   {
      public override Value Evaluate() => null;

      public override VerbPresidenceType Presidence => VerbPresidenceType.NotApplicable;

      public override string ToString() => "...";
   }
}
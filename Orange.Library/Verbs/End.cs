using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class End : Verb
   {
      public override Value Evaluate() => null;

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => ";";

      public override int OperandCount => 0;
   }
}
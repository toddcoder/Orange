using Orange.Library.Values;
using static System.Math;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class Power : TwoValueVerb
   {
      public override Value Evaluate(Value x, Value y) => Pow(x.Number, y.Number);

      public override string Location => "Power";

      public override string Message => "pow";

      public override string ToString() => "**";

      public override VerbPresidenceType Presidence => VerbPresidenceType.Power;

      public override bool LeftToRight => false;
   }
}
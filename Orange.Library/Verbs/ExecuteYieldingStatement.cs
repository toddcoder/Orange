using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Verbs
{
   public class ExecuteYieldingStatement : Verb
   {
      Generator generator;

      public ExecuteYieldingStatement(IYieldingStatement statement) => generator = statement.GetGenerator();

      public override Value Evaluate() => generator.Next();

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;
   }
}
using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Parsers
{
   public class MatchExpressionParser : MatchParser
   {
      public MatchExpressionParser()
         : base("^ /(|sp| 'match') /b", VerbPresidenceType.Push)
      {
      }
   }
}
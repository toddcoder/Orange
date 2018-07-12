using static Orange.Library.Managers.ExpressionManager;

namespace Orange.Library.Parsers
{
   public class WithExpressionParser : WithParser
   {
      public WithExpressionParser()
         : base("^ /(' '* 'with') /b", VerbPresidenceType.Push)
      {

      }
   }
}
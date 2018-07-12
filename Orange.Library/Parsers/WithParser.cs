using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;

namespace Orange.Library.Parsers
{
   public class WithParser : Parser
   {
      VerbPresidenceType presidence;

      public WithParser(string pattern = "^ /(|tabs| 'with') /b", VerbPresidenceType presidence = VerbPresidenceType.Statement)
         : base(pattern) => this.presidence = presidence;

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         if (GetExpressionThenBlock(source, NextPosition).If(out var sourceBlock, out var actionsBlock, out var index))
         {
            overridePosition = index;
            return new With(sourceBlock, actionsBlock, presidence) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "with";
   }
}
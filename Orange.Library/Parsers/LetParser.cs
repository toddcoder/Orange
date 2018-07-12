using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ComparisandParser;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class LetParser : Parser
   {
      public LetParser()
         : base("^ /s* 'let' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);

         var tuple = GetComparisand(source, NextPosition, PassAlong("^ |sp| '='"));
         if (tuple.If(out var comparisand, out var condition, out var index) &&
            GetExpression(source, index, EndOfLineConsuming()).If(out var expression, out var i))
         {
            overridePosition = i;
            return new Let(comparisand, condition, expression) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "let";
   }
}
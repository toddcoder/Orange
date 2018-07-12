using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ComparisandParser;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers.Special
{
   public class CaseParser : SpecialParser<Verb>
   {
      public override IMaybe<(Verb, int)> Parse(string source, int index)
      {
         if (freeParser.Scan(source, index, "^ |tabs| 'case' /b"))
         {
            freeParser.ColorAll(KeyWords);
            var pComparisand = GetComparisand(source, freeParser.Position, PassAlong(REGEX_DO_OR_END, false));
            if (pComparisand.If(out var comparisand, out var condition, out var cmpIndex) &&
               GetOneOrMultipleBlock(source, cmpIndex).If(out var resultBlock, out var blkIndex))
            {
               Verb verb = new CaseExecute(comparisand, resultBlock, false, condition);
               return (verb, blkIndex).Some();
            }
         }

         return none<(Verb, int)>();
      }
   }
}
using System.Collections.Generic;
using Orange.Library.Parsers.Line;

namespace Orange.Library.Parsers
{
   public class PostfixOperatorParser : MultiParser
   {
      bool asStatement;

      public PostfixOperatorParser(bool asStatement) => this.asStatement = asStatement;

      public override IEnumerable<Parser> Parsers
      {
         get
         {
            if (asStatement)
               yield return new SetterParser();
            yield return new SendMessageParser(true);
            yield return new PostIncrementDecrementParser();
            yield return new OpenEndRangeParser();
            yield return new LazyOperatorParser();
            yield return new DereferenceParser();
            yield return new IndexerParser();
            yield return new SomeOperatorParser();
            yield return new SingleValueArrayParser();
            yield return new ValueMessageParser();
            yield return new ApplyInvokeParser();
         }
      }
   }
}
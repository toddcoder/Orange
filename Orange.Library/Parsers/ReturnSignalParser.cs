using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class ReturnSignalParser : Parser
   {
      public ReturnSignalParser()
         : base("^ |tabs| 'return' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         if (GetExpression(source, NextPosition, EndOfLine()).If(out var block, out var index))
         {
            overridePosition = index;
            return new ReturnSignal(block) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "return signal";
   }
}
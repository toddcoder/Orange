using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class StopParser : Parser
   {
      public StopParser()
         : base("^ |tabs| 'stop' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);

         if (GetExpression(source, NextPosition, EndOfLineConsuming()).If(out var expression, out var index))
         {
            overridePosition = index;
            return new Verbs.Stop(expression);
         }

         return null;
      }

      public override string VerboseName => "Stop";
   }
}
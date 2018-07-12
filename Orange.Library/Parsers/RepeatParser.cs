using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;

namespace Orange.Library.Parsers
{
   public class RepeatParser : Parser
   {
      public RepeatParser()
         : base("^ |tabs| 'repeat' /s+") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         if (GetExpressionThenBlock(source, NextPosition).If(out var expression, out var block, out var index))
         {
            overridePosition = index;
            var parameters = new Parameters(new[] { "_" });
            return new ForExecute(parameters, expression, block);
         }

         return null;
      }

      public override string VerboseName => "repeat";
   }
}
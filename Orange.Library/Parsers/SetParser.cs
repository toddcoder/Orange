using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class SetParser : Parser
   {
      public SetParser()
         : base(@"^' '* '%('") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var block, out var index))
         {
            overridePosition = index;
            return new CreateSet(block);
         }

         return null;
      }

      public override string VerboseName => "create set";
   }
}
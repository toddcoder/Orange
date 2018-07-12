using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class InvokeParser : Parser
   {
      public InvokeParser()
         : base("^ /(':'? '(')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(length, Structures);

         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var block, out var index))
         {
            var arguments = new Arguments(block);
            overridePosition = index;

            return tokens[1].StartsWith(":") ? (Verb)new ApplyInvoke(arguments) : new Invoke(arguments);
         }

         return null;
      }

      public override string VerboseName => "invoke";
   }
}
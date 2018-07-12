using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class MacroLiteralParser : Parser
   {
      public MacroLiteralParser()
         : base("^ ' '* '.' /(['({'])") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         if (GetExpression(source, NextPosition, Close()).If(out var block, out var index))
         {
            result.Value = block;
            var macro = new Macro();
            overridePosition = index;
            return new Push(macro);
         }

         return null;
      }

      public override string VerboseName => "macro literal";
   }
}
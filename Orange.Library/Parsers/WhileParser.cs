using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class WhileParser : Parser
   {
      public WhileParser()
         : base("^ /(|tabs| 'while' | 'until') /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var type = tokens[1].Trim();
         Color(position, tokens[1].Length, KeyWords);
         if (GetExpressionThenBlock(source, NextPosition).If(out var condition, out var block, out var index))
         {
            overridePosition = index;
            return new WhileExecute(condition, block, type == "while") { Index = position };
         }

         return null;
      }

      public override string VerboseName => "while";
   }
}
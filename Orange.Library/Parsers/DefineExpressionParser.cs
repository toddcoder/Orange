using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class DefineExpressionParser : Parser
   {
      public DefineExpressionParser()
         : base($"^ /(|tabs| 'let' /s+) /({REGEX_VARIABLE}) /(/s* '=' /s*)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var fieldName = tokens[2];

         Color(position, tokens[1].Length, KeyWords);
         Color(fieldName.Length, Variables);
         Color(tokens[3].Length, Operators);

         if (GetExpression(source, NextPosition, EndOfLine()).If(out var block, out var i))
         {
            overridePosition = i;
            var thunk = new Thunk(block);
            result.Value = thunk;
            return new DefineExpression(fieldName, thunk) { Index = NextPosition };
         }

         return null;
      }

      public override string VerboseName => "define expression";
   }
}
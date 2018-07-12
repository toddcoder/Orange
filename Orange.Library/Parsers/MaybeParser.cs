using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;
using Maybe = Orange.Library.Verbs.Maybe;

namespace Orange.Library.Parsers
{
   public class MaybeParser : Parser
   {
      const string REGEX_GUARD_OR_END = "(^ ' '* 'guard' /b) | (^ /r /n | ^ /r | ^ /n)";

      FreeParser freeParser;
      EndOfLineParser endOfLineParser;

      public MaybeParser()
         : base($"^ /(|tabs| 'maybe' /s*) /({REGEX_VARIABLE}) /(/s* '=' /s*)")
      {
         freeParser = new FreeParser();
         endOfLineParser = new EndOfLineParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var fieldName = tokens[2];
         Color(position, tokens[1].Length, KeyWords);
         Color(fieldName.Length, Variables);
         Color(tokens[3].Length, Structures);

         if (GetExpression(source, NextPosition, PassAlong(REGEX_GUARD_OR_END, false)).If(out var expression, out var index))
         {
            var currentIndex = index;
            var guardBlock = none<Block>();
            if (freeParser.Scan(source, currentIndex, "^ |sp| 'guard' /b"))
            {
               freeParser.ColorAll(KeyWords);
               currentIndex = freeParser.Position;
               if (GetExpression(source, currentIndex, EndOfLine()).If(out var guard, out var i))
               {
                  currentIndex = i;
                  guardBlock = guard.Some();
               }
            }
            if (endOfLineParser.Scan(source, currentIndex))
               currentIndex = endOfLineParser.Position;
            if (GetBlock(source, currentIndex, true).If(out var ifTrue, out var j))
            {
               currentIndex = j;
               var ifFalse = none<Block>();
               if (guardBlock.IsNone && freeParser.Scan(source, currentIndex, "^ /(|tabs| 'else') (/r /n | /r | /n) "))
               {
                  freeParser.ColorAll(KeyWords);
                  currentIndex = freeParser.Position;
                  if (GetBlock(source, currentIndex, true).If(out var elseBlock, out var elseIndex))
                  {
                     currentIndex = elseIndex;
                     guardBlock = elseBlock.Some();
                  }
               }
               if (guardBlock.IsSome)
                  Assert(ifTrue.LastIsReturn, "Maybe", "return required");
               overridePosition = currentIndex;
               return new Maybe(fieldName, expression, ifTrue, ifFalse, guardBlock) { Index = position };
            }
         }

         return null;
      }

      public override string VerboseName => "maybe";
   }
}
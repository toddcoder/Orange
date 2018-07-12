using Orange.Library.Parsers.Special;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MatchParser : Parser
   {
      protected CaseParser caseParser;
      protected CodeBuilder builder;

      public MatchParser()
         : base($"^ /(|tabs|) /'match' /b (/(/s+) /({REGEX_VARIABLE}) /(/s* '='))?")
      {
         caseParser = new CaseParser();
         builder = new CodeBuilder();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var fieldName = tokens[4];
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);
         Color(tokens[3].Length, Whitespaces);
         Color(fieldName.Length, Variables);
         Color(tokens[5].Length, Structures);

         var stop = EndOfLineConsuming();

         if (GetExpression(source, NextPosition, stop).If(out var target, out var index))
         {
            AdvanceTabs();
            while (index < source.Length)
               if (caseParser.Parse(source, index).If(out var verb, out var newIndex))
               {
                  index = newIndex;
                  builder.Verb(verb);
               }
               else
                  break;

            RegressTabs();
            overridePosition = index;
            return new MatchExecute(target, builder.Block, fieldName) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "match";
   }
}
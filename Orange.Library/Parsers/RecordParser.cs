using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class RecordParser : Parser
   {
      class OneLineMemberParser : Parser
      {
         public OneLineMemberParser()
            : base($"^ /(|sp|) /({REGEX_VARIABLE}) /(/s* '=' /s*)") { }

         public override Verb CreateVerb(string[] tokens)
         {
            var memberName = tokens[2];

            Color(position, tokens[1].Length, Whitespaces);
            Color(memberName.Length, Variables);
            Color(tokens[3].Length, Structures);

            if (GetExpression(source, NextPosition, CommaOrCloseParenthesis()).If(out var exp, out var index))
            {
               overridePosition = index;
               MemberName = memberName;
               Thunk = new Thunk(exp);
               return new NullOp();
            }

            return null;
         }

         public override string VerboseName => "one line member parser";

         public string MemberName { get; set; }

         public Thunk Thunk { get; set; }
      }

      class MultiLineMemberParser : Parser
      {
         public MultiLineMemberParser()
            : base($"^ /(|tabs|) /({REGEX_VARIABLE}) /(/s* '=' /s*)") { }

         public override Verb CreateVerb(string[] tokens)
         {
            var memberName = tokens[2];

            Color(position, tokens[1].Length, Whitespaces);
            Color(memberName.Length, Variables);
            Color(tokens[3].Length, Structures);

            if (GetExpression(source, NextPosition, EndOfLineConsuming()).If(out var exp, out var index))
            {
               overridePosition = index;
               MemberName = memberName;
               Thunk = new Thunk(exp);
               return new NullOp();
            }

            return null;
         }

         public override string VerboseName => "one line member parser";

         public string MemberName { get; set; }

         public Thunk Thunk { get; set; }
      }

      FreeParser freeParser;

      public RecordParser()
         : base($"^ /(|sp|) /'('? /'rec' (/(/s+ 'of' /s+) /({REGEX_VARIABLE}))? ") => freeParser = new FreeParser();

      public override Verb CreateVerb(string[] tokens)
      {
         var sourceRecord = tokens[5];
         var oneLine = tokens[2] == "(";
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Structures);
         Color(tokens[3].Length, KeyWords);
         Color(tokens[4].Length, KeyWords);
         Color(sourceRecord.Length, Variables);

         var index = NextPosition;
         var members = new Hash<string, Thunk>();

         if (oneLine)
         {
            var parser = new OneLineMemberParser();
            while (index < source.Length && parser.Scan(source, index))
            {
               members[parser.MemberName] = parser.Thunk;
               if (freeParser.Scan(source, parser.Position, "^ |sp| ','"))
               {
                  index = freeParser.Position;
                  continue;
               }

               if (freeParser.Scan(source, parser.Position, "^ |sp| ')'"))
               {
                  overridePosition = freeParser.Position;
                  return new CreateRecord(members, sourceRecord);
               }
            }
         }
         else
         {
            var endOfLineParser = new EndOfLineParser();
            if (endOfLineParser.Scan(source, index))
               index = endOfLineParser.Position;

            AdvanceTabs();

            var parser = new MultiLineMemberParser();
            while (index < source.Length && parser.Scan(source, index))
            {
               index = parser.Position;
               members[parser.MemberName] = parser.Thunk;
            }

            RegressTabs();

            overridePosition = index;
            return new CreateRecord(members, sourceRecord);
         }

         return null;
      }

      public override string VerboseName => "record";
   }
}
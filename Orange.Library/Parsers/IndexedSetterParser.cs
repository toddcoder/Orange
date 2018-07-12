using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class IndexedSetterParser : Parser
   {
      public IndexedSetterParser()
         : base($"^ /(|tabs|) /({REGEX_VARIABLE}) /('$'{REGEX_VARIABLE} | '[+'?)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var fieldName = tokens[2];
         var type = tokens[3];

         Color(position, tokens[1].Length, Whitespaces);
         Color(fieldName.Length, Variables);

         if (type.StartsWith("$"))
         {
            Color(type.Length, Messaging);
            var argumentExp = PushValue(type.Skip(1));
            var assignParser = new AssignParser();
            if (assignParser.Parse(source, NextPosition).If(out var assignment1, out var index1))
            {
               overridePosition = index1;
               return new IndexedSetter(fieldName, argumentExp, assignment1.Verb, assignment1.Expression, false) { Index = position };
            }

            return null;
         }

         Color(type.Length, Structures);
         var insert = type.EndsWith("+");

         if (GetExpression(source, NextPosition, CloseBracket(), true).If(out var assignment2, out var index2))
         {
            var assignParser = new AssignParser();
            if (assignParser.Parse(source, index2).If(out var assignment3, out var index3))
            {
               overridePosition = index3;
               return new IndexedSetter(fieldName, assignment2, assignment3.Verb, assignment3.Expression, insert) { Index = position };
            }
         }

         return null;
      }

      public override string VerboseName => "indexed setter";
   }
}
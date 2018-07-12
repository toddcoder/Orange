using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using Standard.Types.Strings;

namespace Orange.Library.Parsers
{
   public class IndexedGetterParser : Parser
   {
      public IndexedGetterParser()
         : base($"^ /(' '*) /({REGEX_VARIABLE}) /('$'{REGEX_VARIABLE} | '[')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var fieldName = tokens[2];
         var type = tokens[3];

         Color(position, tokens[1].Length, Whitespaces);
         Color(fieldName.Length, Variables);

         if (type.StartsWith("$"))
         {
            Color(type.Length, Messaging);
            var arguments = new Arguments(type.Skip(1));
            return new SendMessageToField(fieldName, GetterName("item"), arguments, VerbPresidenceType.SendMessage);
         }

         Color(1, Structures);

         if (GetExpression(source, NextPosition, CloseBracket(), true).If(out var expression, out var index))
         {
            overridePosition = index;
            var arguments = new Arguments(expression);
            return new SendMessageToField(fieldName, GetterName("item"), arguments, VerbPresidenceType.SendMessage) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "indexed getter";
   }
}
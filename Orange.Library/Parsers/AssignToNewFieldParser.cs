using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;

namespace Orange.Library.Parsers
{
   public class AssignToNewFieldParser : Parser
   {
      public AssignToNewFieldParser()
         : base("^ /(|tabs|) /(('public' | 'hidden' | 'imported' | 'temp') /s+)? /('var' | 'val') /(/s+)" +
            $" /({REGEX_VARIABLE}) /(/s* '=' /s*)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var visibility = ParseVisibility(tokens[2].Trim());
         var type = tokens[3];
         var fieldName = tokens[5];
         var readOnly = type == "val";

         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);
         Color(type.Length, KeyWords);
         Color(tokens[4].Length, Whitespaces);
         Color(fieldName.Length, Variables);
         Color(tokens[6].Length, Structures);

         if (GetExpression(source, NextPosition, EndOfLine()).If(out var expression, out var index))
         {
            overridePosition = index;
            return new AssignToNewField(fieldName, readOnly, expression, visibility) { Index = NextPosition };
         }

         return null;
      }

      public override string VerboseName => "assign to new field";
   }
}
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;

namespace Orange.Library.Parsers
{
   public class CreateFieldParser : Parser
   {
      FreeParser parser;

      public CreateFieldParser()
         : base("^ /(|tabs|) /(('public' | 'private' | 'protected' | 'temp') /s+)? /('var' | 'val') /(/s+)" +
              $" /({REGEX_VARIABLE})") => parser = new FreeParser();

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
         if (parser.Scan(source, NextPosition, "^ /s* ','"))
         {
            var fieldListParser = new FieldListParser();
            if (fieldListParser.Parse(source, parser.Position).If(out var fieldNames, out var index))
            {
               overridePosition = index;
               var fields = new string[fieldNames.Length + 1];
               System.Array.Copy(fieldNames, 0, fields, 1, fieldNames.Length);
               fields[0] = fieldName;
               return new CreateFields(readOnly, fields, visibility) { Index = position };
            }
            return null;
         }
         return new CreateField(readOnly, fieldName, visibility) { Index = position };
      }

      public override string VerboseName => "create variable";
   }
}
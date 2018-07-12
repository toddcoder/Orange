using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class DataParser : Parser
   {
      public DataParser()
         : base($"^ /(|tabs|) /'type' /(/s+) /({REGEX_VARIABLE}) /(/s* '=' /s*)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var name = tokens[4];
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);
         Color(tokens[3].Length, Whitespaces);
         Color(name.Length, Types);
         Color(tokens[5].Length, Structures);

         var constructors = new Hash<string, int>();

         var constructorParser = new ConstructorParser();

         var index = NextPosition;
         while (index < source.Length)
            if (constructorParser.Parse(source, index).If(out var data, out index))
            {
               constructors[data.Name] = data.Count;
               if (!data.Trailing)
                  break;
            }
            else
               return null;

         overridePosition = index;

         return new NewData(name, constructors);
      }

      public override string VerboseName => "data";
   }
}
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Enumerations
{
   public class NumberEnumerationNameParser : Parser, IEnumerationParser
   {
      IntegerParser integerParser;
      HexParser hexParser;

      public NumberEnumerationNameParser()
         : base($"^ /(/s*) /({REGEX_VARIABLE}) /(/s* '=' /s*)")
      {
         integerParser = new IntegerParser();
         hexParser = new HexParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, EntityType.Whitespaces);
         var name = tokens[2];
         Color(name.Length, EntityType.Variables);
         Color(tokens[3].Length, EntityType.Operators);
         var index = position + length;
         if (integerParser.Scan(source, index))
         {
            Builder.Add(name, (int)integerParser.Result.Value.Number);
            overridePosition = integerParser.Result.Position;
            return new NullOp();
         }
         if (hexParser.Scan(source, index))
         {
            Builder.Add(name, (int)hexParser.Result.Value.Number);
            overridePosition = hexParser.Result.Position;
            return new NullOp();
         }
         return null;
      }

      public override string VerboseName => "number enumeration name";

      public EnumerationBuilder Builder
      {
         get;
         set;
      }
   }
}
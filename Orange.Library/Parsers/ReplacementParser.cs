using System.Collections.Generic;
using System.Linq;
using Orange.Library.Parsers.Replacements;
using Orange.Library.Replacements;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Values.Value;

namespace Orange.Library.Parsers
{
   public class ReplacementParser : Parser, IReplacementParser
   {
      public ReplacementParser()
         : base("^ /s* /('->' | '=:')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var immediate = tokens[1] == "=:";
         Color(position, length, Structures);

         var parsers = new List<Parser>
         {
            new StringParser(),
            new InterpolatedStringParser2(),
            new AssignToAnsReplacementParser(),
            new ValueReplacementParser(),
            new PrintReplacementParser(),
            new BlockReplacementParser(),
            new AtReplacementParser(),
            new PushReplacementParser(),
            new AssignReplacementParser()
         };

         var index = position + length;
         foreach (var parser in parsers.Where(parser => parser.Scan(source, index)))
         {
            if (parser is IReplacementParser replacementParser)
            {
               Replacement = replacementParser.Replacement;
               Replacement.Immediate = immediate;
               overridePosition = parser.Result.Position;
               return new NullOp();
            }

            var value = parser.Result.Value;
            if (value != null)
            {
               switch (value.Type)
               {
                  case ValueType.String:
                     Replacement = new StringReplacement((String)value);
                     break;
                  default:
                     return null;
               }

               overridePosition = parser.Result.Position;
               return new NullOp();
            }
         }

         return null;
      }

      public override string VerboseName => "replacement";

      public IReplacement Replacement { get; set; }
   }
}
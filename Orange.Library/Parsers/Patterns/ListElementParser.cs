using Orange.Library.Patterns;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class ListElementParser : Parser, IElementParser
   {
      public ListElementParser()
         : base(@"^ /s* '\' /s*")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var tokens0Length = tokens[0].Length;
         var index = position + tokens0Length;
         var parser = new PatternParser();
         if (parser.Scan(source, index))
         {
            var delimiterPattern = (Pattern)parser.Result.Value;
            index = parser.Result.Position;
            if (parser.Scan(source, index))
            {
               var mainPattern = (Pattern)parser.Result.Value;
               overridePosition = parser.Result.Position;
               Element = new ListElement(delimiterPattern, mainPattern);
               Color(position, tokens0Length, Operators);
               return new NullOp();
            }
         }
         return null;
      }

      public override string VerboseName => "List element";

      public Element Element
      {
         get;
         set;
      }
   }
}
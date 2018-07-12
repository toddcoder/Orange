using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class FieldDelimiterElementParser : Parser, IElementParser
   {
      public FieldDelimiterElementParser()
         : base("^ /s* ','")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         Element = new FieldDelimiterElement();
         return new NullOp();
      }

      public override string VerboseName => "field delimiter";

      public Element Element
      {
         get;
         set;
      }
   }
}
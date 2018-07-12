using Orange.Library.Patterns;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers.Patterns
{
   public class RemElementParser : Parser, IElementParser
   {
      public RemElementParser()
         : base("^ /s* '%'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, IDEColor.EntityType.Operators);
         Element = new RemElement();
         return new NullOp();
      }

      public override string VerboseName => "rem";

      public Element Element
      {
         get;
         set;
      }
   }
}
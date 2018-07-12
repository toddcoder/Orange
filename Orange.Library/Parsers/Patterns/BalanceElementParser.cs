using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class BalanceElementParser : Parser, IElementParser
   {
      public BalanceElementParser()
         : base("^ /s* '(=)'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         Element = new BalanceElement();
         return new NullOp();
      }

      public override string VerboseName => "balance element";

      public Element Element
      {
         get;
         set;
      }
   }
}
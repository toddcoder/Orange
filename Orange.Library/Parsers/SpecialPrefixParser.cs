using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class SpecialPrefixParser : Parser
   {
      public SpecialPrefixParser()
         : base("^ /(|sp| ['+~^!']) -(> ' '+)")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         var sign = tokens[1].Trim();
         switch (sign)
         {
            case "+":
               return new Plus();
            case "~":
               return new Stringify();
            case "^":
               return new RangeOperator();
            case "!":
               return new CreateGenerator();
            default:
               return null;
         }
      }

      public override string VerboseName => "change sign";
   }
}
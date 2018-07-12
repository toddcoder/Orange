using Orange.Library.Values;
using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
   public class BooleanParser : Parser
   {
      public BooleanParser()
         : base(@"^\s*([01]|-1|\*)\?")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, IDEColor.EntityType.Boolean);
         string type = tokens[1];
         switch (type)
         {
            case "-1":
               result.Value = new Nil();
               break;
            case "*":
               result.Value = new Any();
               break;
            default:
               result.Value = type == "1";
               break;
         }
         return new Push(result.Value);
      }

      public override string VerboseName
      {
         get
         {
            return "boolean";
         }
      }
   }
}
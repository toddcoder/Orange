using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using Ignore = Orange.Library.Values.Ignore;

namespace Orange.Library.Parsers
{
   public class SpecialValueParser : Parser
   {
      public SpecialValueParser()
         : base("^ ' '* /('false' | 'true' | 'nil' | 'any' | 'null' | 'none' | 'ignore') /b")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         var word = tokens[1].ToLower();
         Value value;
         switch (word)
         {
            case "false":
               value = false;
               break;
            case "true":
               value = true;
               break;
            case "nil":
               value = new Nil();
               break;
            case "any":
               value = new Any();
               break;
            case "null":
               value = new Null();
               break;
            case "none":
               value = new None();
               break;
            case "ignore":
               value = new Ignore();
               break;
            default:
               return null;
         }
         result.Value = value;
         return new Push(value);
      }

      public override string VerboseName => "special character";
   }
}
using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class AnyClassElementParser : Parser, IElementParser
   {
      public AnyClassElementParser()
         : base("^ /s* /('++' | '--' | '+' | '-' | /d+)? /('`' ['a-z']1%2)", true)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         var type = tokens[1];
         var text = tokens[2];
         switch (text.ToLower().Substring(1))
         {
            case "s":
               text = STRING_SPACES;
               break;
            case "w":
               text = STRING_WORDS;
               break;
            case "d":
               text = STRING_DIGITS;
               break;
            case "p":
               text = STRING_PUNCT;
               break;
            case "a":
               text = STRING_LETTERS;
               break;
            case "t":
               text = STRING_TAB;
               break;
            case "rn":
               text = STRING_CRLF;
               break;
            case "r":
               text = STRING_CR;
               break;
            case "n":
               text = STRING_LF;
               break;
            case "u":
               text = STRING_UPPER;
               break;
            case "l":
               text = STRING_LOWER;
               break;
            case "v":
               text = STRING_VOWELS;
               break;
            case "uv":
               text = STRING_UVOWELS;
               break;
            case "lv":
               text = STRING_LVOWELS;
               break;
            case "uc":
               text = STRING_UCONSONANTS;
               break;
            case "lc":
               text = STRING_LCONSONANTS;
               break;
            case "q":
               text = STRING_QUOTES;
               break;
            default:
               return null;
         }
         switch (type)
         {
            case "+":
               Element = new SpanElement(text);
               break;
            case "-":
               Element = new BreakElement(text);
               break;
            case "++":
               Element = new MSpanElement(text);
               break;
            case "--":
               Element = new MBreakElement(text);
               break;
            case "":
               Element = new AnyElement(text, 1);
               break;
            default:
               if (text.IsNumeric())
                  Element = new AnyElement(text, text.ToInt());
               else
                  return null;
               break;
         }
         return new NullOp();
      }

      public override string VerboseName => "any class element";

      public Element Element
      {
         get;
         set;
      }
   }
}
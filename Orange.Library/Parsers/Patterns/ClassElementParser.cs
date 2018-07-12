using Orange.Library.Patterns;
using Orange.Library.Patterns2;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class ClassElementParser : Parser, IElementParser, IInstructionParser
   {
      public ClassElementParser()
         : base("^ /(/s*) /('-'? /d+)? /('!')? /(['+@#&$.'])") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Numbers);
         Color(tokens[3].Length, Operators);
         Color(tokens[4].Length, Operators);

         var count = tokens[2].ToInt();
         var span = "";
         var elementNotSet = true;
         var not = tokens[3] == "!";

         switch (tokens[4])
         {
            case "+":
               span = STRING_SPACES;
               break;
            case "@":
               span = STRING_WORDS;
               break;
            case "#":
               span = STRING_DIGITS;
               break;
            case "&":
               span = STRING_PUNCT;
               break;
            case "$":
               span = STRING_LETTERS;
               break;
            case ".":
               if (count == 0)
                  count = 1;
               Element = new LengthElement(count);
               Instruction = new LengthInstruction(count);
               elementNotSet = false;
               break;
            default:
               return null;
         }

         if (elementNotSet)
            if (not)
               Element = count > 0 ? (Element)new AnyElement(span, count) { Not = true } : new BreakElement(span);
            else
               Element = count > 0 ? (Element)new AnyElement(span, count) : new SpanElement(span);

         return new NullOp();
      }

      public override string VerboseName => "class element";

      public Element Element { get; set; }

      public Instruction Instruction { get; set; }
   }
}
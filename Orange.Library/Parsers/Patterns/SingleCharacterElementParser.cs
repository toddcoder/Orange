using Orange.Library.Patterns;
using Orange.Library.Patterns2;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Patterns
{
   public class SingleCharacterElementParser : Parser, IElementParser, IInstructionParser
   {
      public SingleCharacterElementParser()
         : base(@"^ /s* /([';:^&!\-'])")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         switch (tokens[1])
         {
            case "-":
               Element = new AbortElement();
               break;
            case ";":
               Element = new FailElement();
               break;
            case ":":
               Element = new FenceElement();
               break;
            case "^":
               Element = new AnchorElement();
               break;
            case "!":
               Element = new NegateElement();
               Instruction = new NegateInstruction();
               break;
            case "&":
               Element = new UseCaseElement();
               break;
            case "\\":
               Element = new RecordSeparatorElement();
               break;
            default:
               return null;
         }
         return new NullOp();
      }

      public override string VerboseName => "single character element";

      public Element Element
      {
         get;
         set;
      }

      public Instruction Instruction
      {
         get;
         set;
      }
   }
}
using Orange.Library.Patterns;
using Orange.Library.Patterns2;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Patterns
{
   public class LengthElementParser : Parser, IElementParser, IInstructionParser
   {
      public LengthElementParser()
         : base("^ /s* '['") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         if (GetExpression(source, NextPosition, CloseBracket()).If(out var block, out var index))
         {
            Element = new LengthBlockElement(block);
            Instruction = new LengthBlockInstruction(block);
            overridePosition = index;
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "length element";

      public Element Element { get; set; }

      public Instruction Instruction { get; set; }
   }
}
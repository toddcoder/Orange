using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library.Parsers
{
   public class ResetParser : Parser, IReturnsBlock
   {
      public ResetParser()
         : base("^ |tabs| 'reset' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         if (GetBlock(source, NextPosition, true).If(out var block, out var index))
         {
            Block = block;
            overridePosition = index;
            return block.PushedVerb;
         }

         return null;
      }

      public override string VerboseName => "reset";

      public Block Block { get; set; }
   }
}
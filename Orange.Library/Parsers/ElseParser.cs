using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library.Parsers
{
   public class ElseParser : Parser, IReturnsBlock
   {
      public ElseParser()
         : base("^ |tabs| 'else' (/r /n | /r | /n)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);
         if (GetBlock(source, NextPosition, true).If(out var block, out var index))
         {
            overridePosition = index;
            Block = block;
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "else";

      public Block Block { get; set; }
   }
}
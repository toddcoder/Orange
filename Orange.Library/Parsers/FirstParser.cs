using Orange.Library.Parsers.Line;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library.Parsers
{
   public class FirstParser : Parser, IReturnsBlock
   {
      protected Block block;

      public FirstParser()
         : base("^ |tabs| 'first' /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, KeyWords);

         var endOfLineParser = new EndOfLineParser();
         endOfLineParser.Scan(source, NextPosition);
         var index = endOfLineParser.Position;
         if (index == -1)
            return null;

         if (GetBlock(source, index, true) is Some<(Block, int)> some)
         {
            (var b, var i) = some.Value;
            overridePosition = i;
            block = b;
            block.AutoRegister = false;
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "each first";

      public Block Block => block;
   }
}
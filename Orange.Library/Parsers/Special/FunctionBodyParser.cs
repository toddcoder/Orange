using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Parser;
using static Orange.Library.Parsers.StatementParser;
using static Standard.Types.Maybe.MaybeFunctions;
using If = Orange.Library.Verbs.If;

namespace Orange.Library.Parsers.Special
{
   public class FunctionBodyParser : SpecialParser<Block>
   {
      public bool ExtractCondition { get; set; }

      public override IMaybe<(Block, int)> Parse(string source, int index)
      {
         if (freeParser.Scan(source, index, "^ /s* '=' /s*"))
         {
            Color(index, freeParser.Length, Structures);
            index = freeParser.Position;
            if (OneLineStatement(source, index).If(out var b1, out var i1))
            {
               var block = b1;
               if (ExtractCondition)
               {
                  var newBlock = createCondition(block);
                  if (newBlock != null)
                     block = newBlock;
               }
               return (block, i1).Some();
            }
         }

         if (ConsumeEndOfLine(source, index).If(out var i2))
            index = i2;

         if (GetBlock(source, index, true).If(out var b2, out var i3))
         {
            MultiCapable = true;
            return (b2, i3).Some();
         }

         return none<(Block, int)>();
      }

      Block createCondition(Block block)
      {
         var blockBuilder = new CodeBuilder();
         var conditionBuilder = new CodeBuilder();
         var buildingCondition = false;
         Condition = null;
         Where = null;
         foreach (var verb in block.AsAdded)
         {
            if (buildingCondition)
            {
               conditionBuilder.Verb(verb);
               continue;
            }

            switch (verb)
            {
               case If _:
                  buildingCondition = true;
                  break;
               case Where where:
                  Where = where.Block;
                  break;
               default:
                  blockBuilder.Verb(verb);
                  break;
            }
         }

         if (buildingCondition)
            Condition = conditionBuilder.Block;
         block = blockBuilder.Block;
         return block;
      }

      public bool MultiCapable { get; set; }

      public Block Condition { get; set; }

      public Block Where { get; set; }
   }
}
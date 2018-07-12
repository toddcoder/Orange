using System;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.Parser;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library
{
   public static class NewOrangeCompiler
   {
      public static Block Compile(string source)
      {
         Tabs = "";
         if (GetBlock(source, 0, false, compileAll: true) is Some<(Block, int)> some)
         {
            (var block, var _) = some.Value;
            while (block.Count > 0)
            {
               var i = block.Count - 1;
               if (block[i] is End)
                  block.RemoveAt(i);
               else
                  break;
            }
            return block;
         }

         throw new ApplicationException("Block not generated");
      }
   }
}
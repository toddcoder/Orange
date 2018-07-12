using Orange.Library.Replacements;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Replacements
{
   public class BlockReplacementParser : Parser, IReplacementParser
   {
      LambdaParser blockParser;

      public BlockReplacementParser()
         : base("^ /(/s*) '('") => blockParser = new LambdaParser();

      public override Verb CreateVerb(string[] tokens)
      {
         var tokens1Length = tokens[1].Length;
         Color(position, tokens1Length, Whitespaces);
         var index = position + tokens1Length;
         Block block;
         Parameters parameters;
         var returnValue = true;
         if (blockParser.Scan(source, index))
         {
            if (blockParser.Value is Lambda lambda)
            {
               block = lambda.Block;
               parameters = lambda.Parameters;
            }
            else
            {
               block = (Block)blockParser.Value;
               parameters = new NullParameters();
            }
            index = blockParser.Result.Position;
         }
         else
            return null;

         if (index < source.Length && source.Substring(index, 1) == ".")
         {
            index++;
            returnValue = false;
         }
         overridePosition = index;
         Replacement = new BlockReplacement(block, parameters, returnValue);
         return new NullOp();
      }

      public override string VerboseName => "block replacement";

      public IReplacement Replacement { get; set; }
   }
}
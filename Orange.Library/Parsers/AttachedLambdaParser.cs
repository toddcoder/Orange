using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class AttachedLambdaParser : Parser
   {
      BlockParser lambdaParser;

      public AttachedLambdaParser()
         : base("^ /(/s* '.') '{' ['|<']")
      {
         lambdaParser = new BlockParser(false);
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var tokens1Length = tokens[1].Length;
         Color(position, tokens1Length, Structures);
         if (lambdaParser.Scan(source, position + tokens1Length))
         {
            result.Value = lambdaParser.Result.Value;
            overridePosition = lambdaParser.Result.Position;
            return new NullOp();
         }
         return null;
      }

      public override string VerboseName => "attached lambda";
   }
}
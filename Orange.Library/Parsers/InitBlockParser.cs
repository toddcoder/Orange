using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Values.Object.VisibilityType;

namespace Orange.Library.Parsers
{
   public class InitBlockParser : Parser
   {
      FunctionBodyParser functionBodyParser;

      public InitBlockParser()
         : base("^ /(|tabs| 'init') /b") => functionBodyParser = new FunctionBodyParser();

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         Color(tokens[2].Length, Structures);
         if (functionBodyParser.Parse(source, NextPosition).If(out var block, out var index))
         {
            overridePosition = index;
            var staticBlock = new Block
            {
               new CreateFunction("__$init", new Lambda(new Region(), block, new Parameters(), false),
                  false, Protected, false, null)
            };
            AddStaticBlock(staticBlock);
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "init block";
   }
}
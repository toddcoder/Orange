using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class IndexerParser : Parser
   {
      static IMaybe<Block> argumentMessageInvokation(Block block)
      {
         if (block.Count == 0)
            return none<Block>();

         var verbs = block.AsAdded;
         var builder = new CodeBuilder();
         builder.Push();
         var messaging = false;
         var invoke = new InvokeMessageArguments();

         for (var i = 0; i < verbs.Count; i++)
         {
            var verb = verbs[i];
            switch (verb)
            {
               case AppendToMessage append when messaging:
                  invoke.AddMessage(append.MessageName);
                  break;
               case AppendToMessage append:
                  builder.PopAndParenthesize();
                  messaging = true;
                  invoke.AddMessage(append.MessageName);
                  break;
               case Push push when messaging:
                  var valueBlock = PushValue(push.Value);
                  invoke.AddValue(valueBlock);
                  break;
               case Push push when i == 0:
                  builder.Verb(new Push(push.Value));
                  break;
               default:
                  builder.Verb(verb);
                  break;
            }
         }

         if (messaging)
         {
            builder.Verb(invoke);
            var newBlock = builder.Block;
            return newBlock.Some();
         }

         return none<Block>();
      }

      public IndexerParser()
         : base("^ /'['") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);

         if (GetExpression(source, NextPosition, CloseBracket(), true).If(out var argumentExp, out var i))
         {
            overridePosition = i;
            return new SendMessage(GetterName("item"), new Arguments(argumentExp));
         }

         return null;
      }

      public override string VerboseName => "indexer";
   }
}
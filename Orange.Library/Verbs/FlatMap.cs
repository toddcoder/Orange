using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class FlatMap : Verb
   {
      const string LOCATION = "Flat map";
      const string MESSAGE_NAME = "fmap";

      public override Value Evaluate()
      {
         var stack = State.Stack;
         var code = stack.Pop(true, LOCATION, false);
         var arrayValue = stack.Pop(true, LOCATION);
         if (arrayValue.IsArray)
         {
            var array = (Array)arrayValue.SourceArray;
            Arguments arguments;
            switch (code)
            {
               case Lambda lambda:
                  arguments = new Arguments(new NullBlock(), lambda.Block, lambda.Parameters);
                  return MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
               case Block block:
                  arguments = new Arguments(new NullBlock(), block, new NullParameters());
                  return MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
               case IStringify _:
                  var stringifyBlock = new Block { new Push(code) };
                  arguments = new Arguments(new NullBlock(), stringifyBlock, new NullParameters());
                  return MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
               case Values.Format format:
                  var formatBlock = new Block { new Push(format.Stringify.String) };
                  arguments = new Arguments(new NullBlock(), formatBlock, format.Parameters);
                  return MessagingState.SendMessage(array, MESSAGE_NAME, arguments);
               default:
                  return arrayValue;
            }
         }

         return arrayValue;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Apply;

      public override string ToString() => "->&";
   }
}
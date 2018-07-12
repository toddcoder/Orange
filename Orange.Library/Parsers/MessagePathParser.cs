using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MessagePathParser : SendMessageParser
   {
      public MessagePathParser(bool useBlockOrLambda)
         : base(useBlockOrLambda, $"^ /(|sp|) /'.^' /({REGEX_VARIABLE}) /('*')? /('(')?")
      {
      }

      protected override Verb getVerb(string type, string message, Arguments arguments, int index) => new Push(getMessagePath(message, arguments, index));

      protected Value getMessagePath(string firstMessage, Arguments arguments, int index)
      {
         var messages = new List<Message>
         {
            new Message(firstMessage, arguments)
         };
         var parser = new SimpleSendMessageParser(blockOrLambdaParser);
         while (parser.Scan(source, index))
         {
            messages.Add((Message)parser.Result.Value);
            index = parser.Result.Position;
         }
         overridePosition = index;
         var messagePath = new MessagePath(messages);
         result.Value = messagePath;
         return messagePath;
      }
   }
}
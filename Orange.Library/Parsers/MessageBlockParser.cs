using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MessageBlockParser : Parser
   {
      public const string REGEX_SEND_MESSAGE = "^ /(/s*) /'^.' /(" + REGEX_VARIABLE + ") /'('?";

      TrimBlockParser blockParser;
      AttachedLambdaParser attachedLambdaParser;
      LambdaParser lambdaParser;

      public MessageBlockParser()
         : base(REGEX_SEND_MESSAGE)
      {
         blockParser = new TrimBlockParser();
         attachedLambdaParser = new AttachedLambdaParser();
         lambdaParser = new LambdaParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var whitespace = tokens[1];
         Color(position, whitespace.Length, Whitespaces);
         var type = tokens[2];
         Color(type.Length, Structures);
         var message = tokens[3];
         Color(message.Length, Messaging);
         Block actualArguments;
         Block executable = null;
         Parameters parameters = null;
         var index = position + length;
         var bracket = tokens[4];
         Lambda lambda = null;
         if (bracket == "(")
         {
            Color(1, Structures);
            int newIndex;
            if (GetExpression(source, index, "").Assign(out actualArguments, out newIndex))
               index = newIndex;
            else
               actualArguments = new Block();
         }
         else
            actualArguments = new Block();
         if (lambdaParser.Scan(source, index))
         {
            var messageClosure = (Lambda)lambdaParser.Result.Value;
            parameters = messageClosure.Parameters;
            executable = messageClosure.Block;
            index = lambdaParser.Result.Position;
         }
         else if (blockParser.Scan(source, index))
         {
            executable = (Block)blockParser.Result.Value;
            index = blockParser.Result.Position;
         }
         if (attachedLambdaParser.Scan(source, index))
         {
            lambda = (Lambda)attachedLambdaParser.Result.Value;
            index = attachedLambdaParser.Result.Position;
         }
         overridePosition = index;
         MessagingState.RegisterMessageCall(message);
         var arguments = new Arguments(actualArguments, executable, parameters);
         if (lambda != null)
            arguments.AddArgument(lambda);
         return new ApplyMessageToDefaultVariable(new Message(message, arguments));
      }

      public override string VerboseName => "message block";
   }
}
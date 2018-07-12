using Orange.Library.Verbs;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class SendMessageToClassParser : SendMessageParser
   {
      public SendMessageToClassParser(bool useBlockOrLambda)
         : base(useBlockOrLambda, $"^ /(' '*) /('@') /({REGEX_VARIABLE}) /('*')? /('(')?")
      {
      }

      protected override Verb getVerb(string type, string message, Arguments arguments, int index) => new SendMessageToClass(message, arguments);
   }
}
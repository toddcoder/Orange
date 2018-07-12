using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers.Line
{
   public class ValueMessageParser : Parser
   {
      public ValueMessageParser()
         : base("^ '!'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Operators);
         return new ValueOperator();
      }

      public override string VerboseName => "value message";
   }
}
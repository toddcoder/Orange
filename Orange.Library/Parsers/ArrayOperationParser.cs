using Orange.Library.Verbs;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class ArrayOperationParser : Parser
   {
      public ArrayOperationParser()
         : base("^ /(/s* '@') /(['&-'])")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, EntityType.Operators);
         string message = null;
         var presidence = VerbPresidenceType.Push;
         switch (tokens[2])
         {
            case "&":
               message = "intersect";
               presidence = VerbPresidenceType.BitAnd;
               break;
            case "-":
               message = "diff";
               presidence = VerbPresidenceType.Subtract;
               break;
         }
         RejectNull(message, VerboseName, $"Didnt' understand @{tokens[2]}");
         return new ArrayOperation(message, presidence);
      }

      public override string VerboseName => "array operation";
   }
}
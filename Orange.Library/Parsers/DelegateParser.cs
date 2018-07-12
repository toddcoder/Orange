using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class DelegateParser : Parser
   {
      public DelegateParser()
         : base("^ /(/s* 'delegate' /s+) /(" + REGEX_VARIABLE + "('.' " + REGEX_VARIABLE + ")?) /(/s+ 'to' /s+) (/(" +
            REGEX_VARIABLE + ")? /('.'))? /(" + REGEX_VARIABLE + ")") { }

      public override Verb CreateVerb(string[] tokens)
      {
         if (!InClassDefinition)
            return null;

         Color(position, tokens[1].Length, KeyWords);
         var delegateMessage = tokens[2];
         Color(delegateMessage.Length, Variables);
         Color(tokens[3].Length, KeyWords);
         var target = tokens[4];
         Color(target.Length, Variables);
         var dot = tokens[5];
         var targetMessage = tokens[6];
         Color(dot.Length, Structures);
         Color(targetMessage.Length, Messaging);
         if (target.IsEmpty() && dot == ".")
            target = "self";
         return new CreateDelegate(delegateMessage, target, targetMessage);
      }

      public override string VerboseName => "delegate";
   }
}
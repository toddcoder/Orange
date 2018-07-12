using System.Collections.Generic;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class DoesParser : Parser
   {
      const string REGEX_TRAIT = "^ /(/s* ',' /s*) /(" + REGEX_VARIABLE + ")";

      public DoesParser()
         : base($"^ /(/s* 'does' /s+) /({REGEX_VARIABLE})") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var keyWordLength = tokens[1].Length;
         Color(position, keyWordLength, KeyWords);
         var firstTrait = tokens[2];
         Color(firstTrait.Length, Types);
         Traits = new List<string> { firstTrait };
         var index = position + length;
         var matcher = new Matcher();
         while (index < source.Length)
            if (matcher.IsMatch(source.Skip(index), REGEX_TRAIT))
            {
               var comma = matcher.FirstGroup;
               var traitName = matcher.SecondGroup;
               Color(comma.Length, Structures);
               Color(traitName.Length, Variables);
               Traits.Add(traitName);
               index += matcher.Length;
            }
            else
               break;

         overridePosition = index;
         return new NullOp();
      }

      public override string VerboseName => "does";

      public List<string> Traits { get; set; }
   }
}
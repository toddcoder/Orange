using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;

namespace Orange.Library.Parsers
{
   public class DefineParser : Parser
   {
      const string REGEX_KEYWORDS = "^ /('var' | 'val' | 'public' | 'private' | 'protected' | 'temp' | 'locked') $";

      public static bool IsDefinedKeyword(string word) => word.IsMatch(REGEX_KEYWORDS);

      public DefineParser()
         : base($"^ /(|sp|) /('public' | 'private' | 'temp' | 'locked')? /(/s*) /('var' | 'val') /(/s+) /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var visibility = tokens[2];
         var visibilityType = ParseVisibility(visibility);
         var readOnly = tokens[4] == "val";
         var variableName = tokens[6];
         Color(position, tokens[1].Length, Whitespaces);
         Color(visibility.Length, KeyWords);
         Color(tokens[3].Length, Whitespaces);
         Color(tokens[4].Length, KeyWords);
         Color(tokens[5].Length, Whitespaces);
         Color(variableName.Length, Variables);
         return new Define(variableName, visibilityType, readOnly);
      }

      public override string VerboseName => "define";
   }
}
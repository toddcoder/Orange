using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Values.Object;

namespace Orange.Library.Parsers
{
   public class ParameterModifierParser : Parser
   {
      public ParameterModifierParser()
         : base("^ /(/s*) /(('public' | 'private' | 'protected' | 'temp' | 'locked') /s+)? /('val')? /b")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var visibility = tokens[2].Trim();
         var readOnly = tokens[3] == "val";
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);
         Color(tokens[3].Length, KeyWords);
         return new ParameterModifiers {VisibilityType = ParseVisibility(visibility), ReadOnly = readOnly};
      }

      public override string VerboseName => "parameter modifier";
   }
}
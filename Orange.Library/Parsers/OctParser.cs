using Orange.Library.Verbs;
using Standard.Types.Strings;
using static System.Math;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class OctParser : Parser
   {
      public OctParser()
         : base("^ ' '* '0o' /{0-7_}", true)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Numbers);
         result.Value = GetNumber(tokens[1]);
         return new Push(result.Value);
      }

      public static int GetNumber(string oct)
      {
         oct = oct.Replace("_", "").Reverse().ToLower();
         var accum = 0;
         for (var i = 0; i < oct.Length; i++)
         {
            var octBase = (int)Pow(8, i);
            var index = "01234567".IndexOf(oct[i]);
            accum += octBase * index;
         }
         return accum;
      }

      public override string VerboseName => "oct";
   }
}
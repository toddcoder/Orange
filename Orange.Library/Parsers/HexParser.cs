using Orange.Library.Verbs;
using Standard.Types.Strings;
using static System.Math;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class HexParser : Parser
   {
      public HexParser()
         : base("^ ' '* '0x' /{a-f0-9_}", true)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Numbers);
         result.Value = GetNumber(tokens[1]);
         return new Push(result.Value);
      }

      public static int GetNumber(string hex)
      {
         hex = hex.Replace("_", "").Reverse().ToLower();
         var accum = 0;
         for (var i = 0; i < hex.Length; i++)
         {
            var hexBase = (int)Pow(16, i);
            var index = "0123456789abcdef".IndexOf(hex[i]);
            accum += hexBase * index;
         }
         return accum;
      }

      public override string VerboseName => "hex";
   }
}
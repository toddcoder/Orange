using Orange.Library.Verbs;
using Standard.Types.Strings;
using static System.Math;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class BinParser : Parser
   {
      public BinParser()
         : base("^ ' '* '0b' /{01_}", true)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Numbers);
         result.Value = GetNumber(tokens[1]);
         return new Push(result.Value);
      }

      public static int GetNumber(string bin)
      {
         bin = bin.Replace("_", "").Reverse().ToLower();
         var accum = 0;
         for (var i = 0; i < bin.Length; i++)
         {
            var binBase = (int)Pow(2, i);
            var index = "01".IndexOf(bin[i]);
            accum += binBase * index;
         }
         return accum;
      }

      public override string VerboseName => "bin";
   }
}
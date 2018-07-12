using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class FreeParser : Parser
   {
      public FreeParser()
         : base("")
      {
      }

      public bool Scan(string source, int index, string pattern, bool ignoreCase = false, bool multiline = false)
      {
         this.pattern = pattern;
         this.ignoreCase = ignoreCase;
         this.multiline = multiline;
         return Scan(source, index);
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Tokens = tokens;
         return new NullOp();
      }

      public string[] Tokens
      {
         get;
         set;
      } = new string[0];

      public void Colorize(params EntityType[] colors)
      {
         if (colors.Length == 0)
            return;

         Assert(Tokens.Length - 1 == colors.Length, "Free parser", "The number tokens and colors must match");

         Color(position, Tokens[1].Length, colors[0]);
         for (var i = 1; i < colors.Length; i++)
            Color(Tokens[i + 1].Length, colors[i]);
      }

      public void ColorAll(EntityType color) => Color(position, length, color);

      public override string VerboseName => "free";
   }
}
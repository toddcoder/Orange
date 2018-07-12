using System.Text;
using Orange.Library.Replacements;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Runtime;
using static Standard.Types.Booleans.Assertions;

namespace Orange.Library.Patterns
{
   public class BreakXElement : Element
   {
      protected static string getPattern(string text)
      {
         var possiblePattern = text;
         if (possiblePattern == "")
            return null;

         possiblePattern = Expand(possiblePattern);
         if (possiblePattern.Has("-"))
            possiblePattern = possiblePattern.Replace("-", "") + "-";

         var pattern = new StringBuilder("-[");
         pattern.Append(possiblePattern.Escape());
         pattern.Append("]");
         pattern.Append("+");
         return pattern.ToString();
      }

      protected string text;
      protected int matchIndex;
      protected int matchCount;

      public BreakXElement(string text, int matchIndex = 0)
      {
         this.text = text;
         this.matchIndex = matchIndex;
      }

      public override bool Evaluate(string input)
      {
         var pattern = getPattern(text);
         if (pattern == null)
            return false;

         index = State.Position;
         var slicer = new Matcher();
         if (slicer.IsMatch(input.Skip(index), pattern, State.IgnoreCase))
         {
            matchCount = slicer.MatchCount;
            if (matchIndex >= matchCount)
               return false;

            var match = slicer.GetMatch(matchIndex);
            if (matchIndex == 0 && match.Index > 0)
               return false;

            length = match.Length + match.Index;
            return true;
         }

         matchCount = 0;
         return false;
      }

      public override Element Alternate
      {
         get
         {
            if (matchCount > 0 && matchIndex >= matchCount)
               return alternate;

            Assert(matchIndex <= MAX_LOOP, "BreakX: infinite loop");
            return new BreakXElement(text, matchIndex + 1) { Next = next, Alternate = alternate };
         }
         set => base.Alternate = value;
      }

      public override Element Clone() => clone(new BreakXElement(text, matchIndex));

      public override IReplacement Replacement
      {
         get => replacement;
         set => setOverridenReplacement(value);
      }
   }
}
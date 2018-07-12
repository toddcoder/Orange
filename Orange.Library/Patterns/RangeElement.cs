using Orange.Library.Replacements;
using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
   public class RangeElement : Element
   {
      int from;
      int to;
      Pattern pattern;
      int count;

      public RangeElement(int from, int to, Pattern pattern, int count = -1)
      {
         this.from = from;
         this.to = to;
         this.pattern = pattern;
         this.pattern.SubPattern = true;
         this.count = count == -1 ? to : count;
      }

      public RangeElement()
         : this(0, 0, new Pattern()) { }

      public override bool Evaluate(string input)
      {
         index = -1;
         var anchored = State.Anchored;
         State.Anchored = true;
         for (var i = 0; i < count; i++)
            if (pattern.Scan(input))
            {
               if (index == -1)
                  index = pattern.Index;
            }
            else
            {
               State.Anchored = anchored;
               return false;
            }

         State.Anchored = anchored;
         length = State.Position - index;
         return true;
      }

      public override Element Alternate
      {
         get
         {
            if (count <= from)
               return alternate;

            return new RangeElement(from, to, pattern, count - 1) { Next = next, Alternate = alternate };
         }
         set => base.Alternate = value;
      }

      public override Element Clone() => clone(new RangeElement(from, to, (Pattern)pattern.Clone()));

      public override string ToString() => $"{from}:{to}({pattern})";

      public override IReplacement Replacement
      {
         get => replacement;
         set => setOverridenReplacement(value);
      }
   }
}
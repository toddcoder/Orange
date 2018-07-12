using Orange.Library.Values;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
   public class ArbNoElement : Element
   {
      protected Pattern pattern;

      public ArbNoElement(Pattern pattern)
      {
         this.pattern = pattern;
         this.pattern.SubPattern = true;
      }

      public ArbNoElement()
         : this(new Pattern())
      {
      }

      public override bool Evaluate(string input)
      {
         var anchored = State.Anchored;
         State.Anchored = true;
         pattern.OwnerNext = next;
         if (!(pattern.Replacement?.Immediate ?? false))
            pattern.OwnerReplacement = pattern.Replacement;
         pattern.SubPattern = true;
         var startIndex = -1;
         var fullLength = 0;
         while (pattern.Scan(input))
         {
            if (startIndex == -1)
               startIndex = pattern.Index;
            fullLength += pattern.Length;
         }
         State.Anchored = anchored;
         if (startIndex == -1)
            return false;
         index = startIndex;
         length = fullLength;
         return true;
      }

      public override string ToString() => $"${pattern}";

      public override bool PositionAlreadyUpdated => false;

      public override Element Clone() => clone(new ArbNoElement((Pattern)pattern.Clone()));

      public override bool AutoOptional => true;
   }
}
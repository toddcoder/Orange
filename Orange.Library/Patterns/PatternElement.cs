using Orange.Library.Replacements;
using Orange.Library.Values;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
   public class PatternElement : Element
   {
      Pattern pattern;

      public PatternElement(Pattern pattern)
      {
         this.pattern = pattern;
         // ReSharper disable once VirtualMemberCallInConstructor
         Replacement = pattern.Replacement;
         pattern.Replacement = null;
      }

      public override bool Evaluate(string input)
      {
         var anchored = State.Anchored;
         State.Anchored = true;
         pattern.OwnerNext = next;
         pattern.SubPattern = true;
         if (pattern.Scan(input))
         {
            index = pattern.Index;
            length = pattern.Length;
            State.Anchored = anchored;
            return true;
         }

         State.Anchored = anchored;
         return false;
      }

      public override IReplacement Replacement { get; set; }

      public override bool PositionAlreadyUpdated => true;

      public override Element Clone() => new PatternElement((Pattern)pattern.Clone())
      {
         Next = cloneNext(),
         Alternate = cloneAlternate(),
         Replacement = cloneReplacement(),
         ID = CompilerState.ObjectID()
      };

      public override string ToString() => pattern.ToString();
   }
}
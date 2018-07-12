using Orange.Library.Replacements;
using static Orange.Library.Runtime;
using static Standard.Types.Booleans.Assertions;

namespace Orange.Library.Patterns
{
   public class ArbElement : Element
   {
      public ArbElement(int length = 0) => this.length = length;

      public override bool Evaluate(string input)
      {
         index = State.Position;
         return length < input.Length;
      }

      public override Element Clone() => clone(new ArbElement());

      public override Element Alternate
      {
         get
         {
            Assert(length <= MAX_LOOP, "Arb infinite loop");

            if (length > State.Input.Length)
               return alternate;

            return new ArbElement(length + 1) { Next = next, Replacement = Replacement, Alternate = alternate };
         }
         set => base.Alternate = value;
      }

      public override string ToString() => "*";

      public override IReplacement Replacement
      {
         get => replacement;
         set => setOverridenReplacement(value);
      }
   }
}
namespace Orange.Library.Patterns
{
   public class LengthElement : Element
   {
      protected int count;

      public LengthElement(int count) => this.count = count;

      public override bool Evaluate(string input)
      {
         var inputLength = input.Length;
         index = Runtime.State.Position;
         length = count;
         return index + length <= inputLength;
      }

      public override Element Clone() => clone(new LengthElement(count));

      public override string ToString() => count == 1 ? "." : count + ".";
   }
}
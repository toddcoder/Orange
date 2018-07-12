namespace Orange.Library.Patterns
{
   public class StringEndElement : Element
   {
      public override bool Evaluate(string input)
      {
         index = Runtime.State.Position;
         var inputLength = input.Length;
         if (Not && index < inputLength || !Not && index >= inputLength)
         {
            length = 0;
            return true;
         }
         return false;
      }

      public override Element Clone() => new StringEndElement
      {
         Next = cloneNext(),
         Alternate = cloneAlternate(),
         Replacement = cloneReplacement()
      };

      public override string ToString() => (Not ? "!" : "") + ">";
   }
}
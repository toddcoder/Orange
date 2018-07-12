namespace Orange.Library.Patterns
{
   public class FieldDelimiterElement : Element
   {
      public override bool Evaluate(string input)
      {
         if (input.Length == 0)
         {
            if (Runtime.State.Multi)
               return false;
            index = Runtime.State.Position;
            length = 0;
            return true;
         }

         var pattern = Runtime.State.FieldPattern;
         var at = pattern.Find(input, Runtime.State.Position, out length);
         if (at == -1)
         {
            if (Runtime.State.Multi)
               return false;
            index = Runtime.State.Position;
            length = 0;
            return true;
         }
         index = at;
         return true;
      }

      public override string ToString() => ",";

      public override Element Clone() => new FieldDelimiterElement
      {
         Next = cloneNext(),
         Alternate = cloneAlternate(),
         Replacement = cloneReplacement()
      };
   }
}
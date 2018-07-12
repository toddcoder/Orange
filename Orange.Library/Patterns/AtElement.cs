namespace Orange.Library.Patterns
{
   public class AtElement : Element
   {
      protected int position;

      public AtElement(int position) => this.position = position;

      public override bool Evaluate(string input)
      {
         if (position < 0)
            position = input.Length - -position + 1;
         index = Runtime.State.Position;
         length = 0;
         return position == index;
      }

      public override Element Clone() => clone(new AtElement(position));

      public override string ToString() => $"@{position}";
   }
}
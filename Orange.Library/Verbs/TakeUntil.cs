namespace Orange.Library.Verbs
{
   public class TakeUntil : TakeWhile
   {
      public override string Message => "takeUntil";

      public override string ToString() => "take until";
   }
}
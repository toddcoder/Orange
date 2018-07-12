namespace Orange.Library.Verbs
{
   public class SkipUntil : TakeWhile
   {
      public override string Message => "skipUntil";

      public override string ToString() => "skip until";
   }
}
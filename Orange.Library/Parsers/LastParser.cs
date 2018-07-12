namespace Orange.Library.Parsers
{
   public class LastParser : FirstParser
   {
      public LastParser() => pattern = "^ |tabs| 'last' /b";

      public override string VerboseName => "each last";
   }
}
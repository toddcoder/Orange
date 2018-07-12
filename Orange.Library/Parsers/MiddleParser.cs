namespace Orange.Library.Parsers
{
   public class MiddleParser : FirstParser
   {
      public MiddleParser() => pattern = "^ |tabs| 'middle' /b";

      public override string VerboseName => "each middle";
   }
}
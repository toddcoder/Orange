using System.Collections.Generic;

namespace Orange.Library.Parsers.Line
{
   public class FillInValueParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            Variable = "";
            yield return new SomeParser();
            yield return new NullOpParser();
            yield return new HexParser();
            yield return new BinParser();
            yield return new OctParser();
            yield return new FloatParser();
            yield return new IntegerParser();
            yield return new StringParser();
            yield return new InterpolatedStringParser2();
            yield return new ArrayLiteralParser();
            yield return new DateParser();
            yield return new FunctionalIfParser();
            yield return new SymbolParser();
            yield return new ListParser();
            yield return new NewObjectParser();
            yield return new RegexParser();
            yield return new EmptyArrayLiteralParser();
            yield return new PatternParser();
            yield return new FormatLiteralParser();
            yield return new SendMessageToClassParser(false);
            yield return new CreateFieldParser();
            yield return new SubstitutionParser(this);
            yield return new VariableParser();
         }
      }

      public string Variable
      {
         get;
         set;
      } = "";
   }
}
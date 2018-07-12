using System.Collections.Generic;

namespace Orange.Library.Parsers.Line
{
   public class ValueParser : MultiParser
   {
      public override IEnumerable<Parser> Parsers
      {
         get
         {
            yield return new EmptyArrayLiteralParser();
            yield return new OuterComprehensionParser();
            //yield return new FunctionalIfParser();
            yield return new ShortLoopRangeParser();
            yield return new LoopRangeParser();
            yield return new AltLoopRangeParser();
            //yield return new LineBlockParser();
            yield return new LambdaBlockParser();
            yield return new DoLambdaParser();
            yield return new LambdaParser();
            yield return new ShortLambdaParser("$(");
            yield return new ParenthesizedExpressionParser();
            yield return new SetParser();
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
            yield return new TypeNameParser();
            yield return new SpecialValueParser();
            yield return new ReferenceParser();
            yield return new SymbolParser();
            yield return new ListParser();
            yield return new NewObjectParser();
            yield return new WithExpressionParser();
            //yield return new MatchExpressionParser();
            yield return new FunctionInvokeParser();
            yield return new RegexParser();
            yield return new MacroLiteralParser();
            yield return new PatternParser();
            yield return new FormatLiteralParser();
            yield return new LazyBlockParser();
            yield return new SendMessageToClassParser(true);
            yield return new IndexedGetterMessageParser();
            yield return new IndexedGetterParser();
            yield return new MessagePathParser(true);
            yield return new VariableParser();
         }
      }
   }
}
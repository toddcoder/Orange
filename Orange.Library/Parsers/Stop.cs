using Standard.Types.Strings;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class Stop
   {
      public static Stop PassAlong(string pattern, bool consume = true, EntityType color = Structures) => new Stop(pattern, consume, color);

      public static Stop PassAlong(Stop stop, EntityType color) => new Stop(stop.Pattern, stop.Consume, color);

      public static Stop EndOfLine() => new Stop(consume: false);

      public static Stop EndOfLineConsuming() => new Stop();

      public static Stop ExpressionThenBlock() => new Stop(REGEX_DO_OR_END, false);

      public static Stop CloseParenthesis() => new Stop("^ /s* ')'");

      public static Stop CloseBracket() => new Stop("']'");

      public static Stop Close() => new Stop("[')}']");

      public static Stop FuncThen() => new Stop("^ |sp| ':'", true, Operators);

      public static Stop FuncElse() => new Stop("(|sp| ')' |sp| 'elseif(') | ('else(') | (')') /b", false);

      public static Stop FuncEnd() => new Stop("^ |sp| ')'", true, Operators);

      public static Stop LoopWhile() => new Stop("('while' | 'until')", false);

      public static Stop LoopThen() => new Stop("'then'", true, KeyWords);

      public static Stop LoopEnd() => new Stop($"^ ' '* ('do' /b | {REGEX_END_OF_LINE})", false);

      public static Stop Yield() => new Stop("^ /s* /(':')");

      public static Stop CommaOrCloseParenthesis() => new Stop("^ |sp| [',)']", false);

      public static Stop Comma() => new Stop("^ |sp| ','");

      public static Stop ComprehensionEnd() => new Stop("^ /s* /(',' | 'if' /b | ':')", false);

      public static Stop WhileOrUntil() => new Stop("^ /s*");

      public static Stop Nothing() => new Stop("^ $");

      public static Stop IfThen() => new Stop("^ |sp| /'then' /b", color: KeyWords);

      public static Stop IfElse() => new Stop("^ |sp| /'else' /b", color: KeyWords);

      public static Stop IfEnd() => new Stop("^ |sp| /'end' /b", color: KeyWords);

      protected Stop(string pattern = "", bool consume = true, EntityType color = Structures)
      {
         if (pattern.IsEmpty())
            Pattern = REGEX_END_OF_LINE;
         else if (!pattern.StartsWith("^ ' '*"))
            Pattern = $"^ ' '* {pattern}";
         else
            Pattern = pattern;
         Color = color;
         Consume = consume;
      }

      public string Pattern { get; }

      public EntityType Color { get; }

      public bool Consume { get; }
   }
}
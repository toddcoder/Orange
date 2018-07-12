using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class ParameterParser : SpecialParser<Parameter>
   {
      static readonly string pattern = "^ /(/s*) /(('public' | 'hidden' | 'inherited' | 'temp' | 'locked') /s+)? " +
         $"/(('val') /s+)? /('?' /s*)? /({REGEX_VARIABLE}) /(/s* '=' /s*)?";

      public override IMaybe<(Parameter, int)> Parse(string source, int index)
      {
         if (freeParser.Scan(source, index, pattern))
         {
            freeParser.Colorize(Whitespaces, KeyWords, KeyWords, Operators, Variables, Structures);
            var tokens = freeParser.Tokens;
            var visibility = ParseVisibility(tokens[2].Trim());
            var readOnly = tokens[3].IsNotEmpty();
            var lazy = tokens[4].IsNotEmpty();
            var parameterName = tokens[5];
            index = freeParser.NextPosition;
            Block defaultValue = null;
            if (tokens[6].IsNotEmpty() && GetExpression(source, index, PassAlong("[',;)']", false)).If(out defaultValue, out var newIndex))
               index = newIndex;
            return (new Parameter(parameterName, defaultValue, visibility, readOnly, lazy), index).Some();
         }

         return none<(Parameter, int)>();
      }
   }
}
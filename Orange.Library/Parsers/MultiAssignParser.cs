using Orange.Library.Parsers.Special;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MultiAssignParser : Parser
   {
      public MultiAssignParser()
         : base($"^ /(|tabs|) /('val' | 'var' | 'set') /(/s*) /'(' /({REGEX_VARIABLE} (/s* ',' /s* {REGEX_VARIABLE})+ ')')" +
            " /(/s* '=' /s*)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var type = tokens[2];
         var readOnly = type == "val";
         var _override = type == "set";
         var whitespaceLength = tokens[3].Length;
         Color(position, tokens[1].Length, Whitespaces);
         Color(type.Length, KeyWords);
         Color(whitespaceLength, Whitespaces);
         Color(tokens[4].Length, Structures);
         var parametersParser = new ParametersParser();
         var offset = tokens[1].Length + type.Length + whitespaceLength + 1;
         if (parametersParser.Parse(source, position + offset).If(out var parameters, out var i) &&
            GetExpression(source, i, EndOfLine()).If(out var expression, out var j))
         {
            overridePosition = j;
            return new MultiAssign(parameters, expression, readOnly, _override) { Index = NextPosition };
         }

         return null;
      }

      public override string VerboseName => "MultiAssign";
   }
}
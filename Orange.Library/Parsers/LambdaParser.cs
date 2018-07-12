using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;
using static Standard.Types.Arrays.ArrayFunctions;

namespace Orange.Library.Parsers
{
   public class LambdaParser : Parser
   {
      ParametersParser parameterParser;

      public LambdaParser()
         : base($"^ /(' '* '(') /({REGEX_VARIABLE} | '_' | '(' {REGEX_VARIABLE} (/s* ',' /s* {REGEX_VARIABLE})+ ')')? " +
              "/(/s*) /('->' | '=>')") => parameterParser = new ParametersParser();

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Structures);
         var param = tokens[2];
         var type = tokens[4];
         Parameters parameters;
         var index = NextPosition;
         if (param == "_")
         {
            Color(param.Length, Structures);
            parameters = new Parameters();
         }
         else if (param.StartsWith("("))
         {
            var leftLength = tokens[1].Length + 1;
            index = position + leftLength;
            Color(position, leftLength, Structures);
            if (!parameterParser.Parse(source, index).If(out parameters, out index))
               return null;
            index = NextPosition;
         }
         else
         {
            Color(param.Length, Variables);
            parameters = new Parameters(array(new Parameter(param)));
         }
         Color(position + tokens[1].Length + tokens[2].Length, tokens[3].Length, Whitespaces);
         Color(type.Length, Structures);
         parameters.Splatting = type == "=>";
         if (GetExpression(source, index, CloseParenthesis()).If(out var block, out var i))
         {
            block.Expression = false;
            var lambda = new Lambda(new Region(), block, parameters, false) { Splatting = parameters.Splatting };
            result.Value = lambda;
            overridePosition = i;
            return new CreateLambda(parameters, block, parameters.Splatting);
         }

         return null;
      }

      public override string VerboseName => "lambda";
   }
}
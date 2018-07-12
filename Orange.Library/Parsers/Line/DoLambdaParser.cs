using System.Linq;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Arrays;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Standard.Types.Arrays.ArrayFunctions;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library.Parsers.Line
{
   public class DoLambdaParser : Parser
   {
      ParameterListParser2 parameterParser;

      public DoLambdaParser()
         : base($"^ /(|sp|) /('do' |sp|) /({REGEX_VARIABLE} | '_' | {REGEX_VARIABLE} (/s* ',' /s* {REGEX_VARIABLE}))? " +
            "/(/r /n | /r | /n)") => parameterParser = new ParameterListParser2
      {
         Pattern = "^ /s* /([',;'] | /r /n | /r | /n)",
         EndOfParameter = "^ /r /n | /r | /n"
      };

      public override Verb CreateVerb(string[] tokens)
      {
         var param = tokens[3];
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, KeyWords);
         var parameterStart = position + tokens.Slice(1, 2).Select(s => s.Length).Sum();

         Parameters parameters;
         var index = NextPosition;

         if (param == "_" || param == "")
         {
            parameters = new Parameters();
            Color(param.Length, Variables);
         }
         else if (param.IsMatch($"^ {REGEX_VARIABLE} $"))
         {
            parameters = new Parameters(array(new Parameter(param)));
            Color(param.Length, Variables);
         }
         else
         {
            if (!parameterParser.Parse(source, parameterStart).If(out var parameterList, out var _))
               return null;

            parameters = new Parameters(parameterList);
         }

         Color(tokens[4].Length, Whitespaces);

         if (GetBlock(source, index, true).If(out var block, out var i))
         {
            index = i;

            block.Expression = false;
            var lambda = new Lambda(new Region(), block, parameters, false) { Splatting = true };
            result.Value = lambda;
            overridePosition = index;
            return new CreateLambda(parameters, block, parameters.Splatting);
         }

         return null;
      }

      public override string VerboseName => "do";
   }
}
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Objects;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.AnyBlockParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Special.ParametersParser;

namespace Orange.Library.Parsers
{
   public class AlternateLambdaParser : Parser
   {
      VariableParser variableParser;
      AnyBlockParser anyBlockParser;

      public AlternateLambdaParser()
         : base(@"^ /s* '\'")
      {
         variableParser = new VariableParser();
         anyBlockParser = new AnyBlockParser(REGEX_LAMBDA_BRIDGE);
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var matcher = new Matcher();
         Color(position, length, Structures);
         var index = position + length;

         var parameters = new Parameters();
         if (variableParser.Scan(source, index))
         {
            var parserResult = variableParser.Result;
            var value = parserResult.Value;
            var variable = value.As<Variable>();
            if (variable.IsSome)
            {
               parameters = new Parameters(new[]
               {
                  new Parameter(variable.Value.Name)
               });
               index = parserResult.Position;
            }
            else
               return null;
         }
         else if (matcher.IsMatch(source.Skip(index), @"^ /s* '('"))
         {
            var matcherLength = matcher[0].Length;
            Color(matcherLength, Structures);
            index += matcherLength;
            var patternParametersParser = new ParametersParser(ParametersType.Pattern);
            if (patternParametersParser.Parse(source, ref index))
               parameters = patternParametersParser.Parameters;
            else
               return null;
         }

         var block = anyBlockParser.Parse(source, ref index, false);
         if (block == null)
            return null;
         var spatting = anyBlockParser.Splatting;
         overridePosition = index;
         return new CreateLambda(parameters, block, spatting);
      }

      public override string VerboseName => "Alternate lambda";
   }
}
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Special.ParametersParser;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object.VisibilityType;

namespace Orange.Library.Parsers
{
   public class EnumerationValueParser : Parser
   {
      const string REGEX_CASE = "^ /(|tabs|) /('value') /(/s+) /(" + REGEX_VARIABLE + ") /(/s* ['=(['] /s*)?";
      const string REGEX_COMMA = "^ /(|sp|) /(',') /(/s+) /(" + REGEX_VARIABLE + ") /(/s* ['=(['] /s*)?";

      IntegerParser integerParser;
      HexParser hexParser;
      bool useCase;

      public EnumerationValueParser(bool useCase = true)
         : base(useCase ? REGEX_CASE : REGEX_COMMA)
      {
         integerParser = new IntegerParser();
         hexParser = new HexParser();
         this.useCase = useCase;
      }

      public override Verb CreateVerb(string[] tokens)
      {
         if (!InClassDefinition)
            return null;

         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, useCase ? KeyWords : Operators);
         Color(tokens[3].Length, Whitespaces);
         var caseName = tokens[4];
         Color(caseName.Length, Variables);
         var tokens5Length = tokens[5].Length;
         var type = tokens[5].Trim();
         Color(tokens5Length, type == "=" ? Operators : Structures);
         var index = position + length;
         var staticFunction = new CodeBuilder();
         Parameters parameters;
         int newIndex;
         if (tokens5Length > 0)
            switch (type)
            {
               case "=":
                  if (integerParser.Scan(source, index))
                  {
                     var parserResult = integerParser.Result;
                     EnumerationValue = (int)parserResult.Value.Number;
                     index = parserResult.Position;
                     staticFunction.ValueAsArgument(EnumerationValue);
                     staticFunction.ValueAsArgument(caseName);
                  }
                  else if (hexParser.Scan(source, index))
                  {
                     var parserResult = hexParser.Result;
                     EnumerationValue = (int)parserResult.Value.Number;
                     index = parserResult.Position;
                     staticFunction.ValueAsArgument(EnumerationValue);
                     staticFunction.ValueAsArgument(caseName);
                  }
                  else
                     return null;

                  break;
               case "(":
                  var parametersParser = new ParametersParser();
                  if (parametersParser.Parse(source, index).If(out parameters, out newIndex))
                  {
                     index = newIndex;
                     foreach (var parameter in parameters.GetParameters())
                     {
                        staticFunction.Parameter(parameter);
                        staticFunction.VariableAsArgument(parameter.Name);
                     }

                     staticFunction.Parameter("value", ++EnumerationValue);
                     staticFunction.VariableAsArgument("value");
                     staticFunction.Parameter("name", caseName);
                     staticFunction.VariableAsArgument("name");
                  }

                  break;
               case "[":
                  staticFunction.ValueAsArgument(++EnumerationValue);
                  staticFunction.ValueAsArgument(caseName);
                  var patternParametersParser = new ParametersParser(ParametersType.Pattern);
                  if (patternParametersParser.Parse(source, index).If(out parameters, out newIndex))
                  {
                     index = newIndex;
                     foreach (var parameter in parameters.GetParameters())
                     {
                        staticFunction.Parameter(parameter);
                        staticFunction.VariableAsArgument(parameter.Name);
                     }
                  }

                  break;
               default:
                  return null;
            }
         else
         {
            staticFunction.ValueAsArgument(++EnumerationValue);
            staticFunction.ValueAsArgument(caseName);
         }

         staticFunction.Variable(ClassName);
         staticFunction.Invoke();

         var lambda = staticFunction.Lambda();
         var staticBlock = new Block { new CreateFunction(caseName, lambda, false, Public, false, null) };
         AddStaticBlock(staticBlock);
         var commaParser = new EnumerationValueParser(false);
         if (commaParser.Scan(source, index))
            index = commaParser.Result.Position;
         overridePosition = index;
         return new NullOp();
      }

      static void setArrayValues(CodeBuilder builder, string caseName)
      {
         RejectNull(builder, "Case parser", "Not called from an enumeration");
         var enumerationValue = new Double(EnumerationValue).Pushed;
         var name = new String(caseName).Pushed;
         builder.IndexedSetterMessage("valueToName", "item", enumerationValue, new NotMatched<Verb>(), name, false);
         builder.IndexedSetterMessage("nameToValue", "item", name, new NotMatched<Verb>(), enumerationValue, false);
      }

      public override string VerboseName => "case";
   }
}
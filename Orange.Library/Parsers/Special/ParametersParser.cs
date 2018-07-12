using System.Collections.Generic;
using Orange.Library.Values;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers.Special
{
   public class ParametersParser : SpecialParser<Parameters>
   {
      public enum ParametersType
      {
         Standard,
         Pattern,
         Message
      }

      ParametersType type;

      public ParametersParser(ParametersType type = ParametersType.Standard) => this.type = type;

      IMaybe<SpecialParser<List<Parameter>>> getParserForType()
      {
         SpecialParser<List<Parameter>> parser;
         switch (type)
         {
            case ParametersType.Standard:
               parser = new ParameterListParser2();
               break;
            case ParametersType.Pattern:
               parser = new PatternParameterListParser2();
               break;
            default:
               return none<SpecialParser<List<Parameter>>>();
         }

         return parser.Some();
      }

      public override IMaybe<(Parameters, int)> Parse(string source, int index)
      {
         if (getParserForType().If(out var parser) && parser.Parse(source, index).If(out var list, out var i))
         {
            var returns = (IReturnsParameterList)parser;
            var parameters = new Parameters(list) { Multi = returns.Multi };
            Currying = returns.Currying;
            return (parameters: parameters, index: i).Some();
         }

         return none<(Parameters, int)>();
      }

      public bool Currying { get; set; }
   }
}
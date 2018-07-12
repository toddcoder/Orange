using System.Collections.Generic;
using Orange.Library.Parsers.Special;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.AnyBlockParser;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class ArrayParametersParser : Parser
   {
      AnyBlockParser blockParser;

      public ArrayParametersParser()
         : base("^ /s* '|('")
      {
         blockParser = new AnyBlockParser(REGEX_STANDARD_BRIDGE);
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         var index = position + length;
         var parametersParser = new ParametersParser();
         if (parametersParser.Parse(source, ref index))
         {
            var parameters = parametersParser.Parameters;
            var block = blockParser.Parse(source, ref index, false);
            overridePosition = index;
            var unpackedVariables = new List<string>();
            if (parameters.Length > 3)
               for (var i = 3; i < parameters.Length; i++)
                  unpackedVariables.Add(parameters[i].Name);
            parameters.Splatting = blockParser.Splatting;
            return new PushArrayParameters(parameters, block, unpackedVariables);
         }
         return null;
      }

      public override string VerboseName => "Array parameters";
   }
}
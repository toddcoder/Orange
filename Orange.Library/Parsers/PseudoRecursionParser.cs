using Orange.Library.Parsers.Special;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class PseudoRecursionParser : Parser
   {
      FunctionBodyParser functionBodyParser;

      public PseudoRecursionParser()
         : base($"^ /(|tabs| 'rec' /s+) /({REGEX_VARIABLE}) /'('") => functionBodyParser = new FunctionBodyParser();

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         var name = tokens[2];
         Color(name.Length, Variables);
         Color(tokens[3].Length, Structures);
         var parametersParser = new ParametersParser();
         if (parametersParser.Parse(source, NextPosition).If(out var parameters, out var i) &&
            functionBodyParser.Parse(source, i).If(out var block, out var j))
         {
            overridePosition = j;
            return new CreatePseudoRecursion(name, parameters, block) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "Pseudo recursion";
   }
}
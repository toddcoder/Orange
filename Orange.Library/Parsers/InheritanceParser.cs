using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class InheritanceParser : Parser
   {
      string variableName;
      Parameters parameters;

      public InheritanceParser()
         : base($"^ /(/s* 'of' /s*) /({REGEX_VARIABLE}) /('(')?") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         variableName = tokens[2];
         var open = tokens[3];
         Color(VariableName.Length, Types);
         Color(open.Length, Structures);
         parameters = null;
         var index = NextPosition;
         if (open == "(")
         {
            InClassDefinition = true;
            Color(open.Length, Structures);
            var parser = new ParametersParser();
            if (parser.Parse(source, index).If(out parameters, out var newIndex))
               index = newIndex;
            else
               return null;

            InClassDefinition = false;
         }

         overridePosition = index;
         return new NullOp();
      }

      public override string VerboseName => "inheritance";

      public string VariableName => variableName;

      public Parameters Parameters => parameters;
   }
}
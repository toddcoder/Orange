using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using Standard.Types.Maybe;

namespace Orange.Library.Parsers
{
   public class GeneratorParser : Parser
   {
      public GeneratorParser()
         : base($"^ /(/s*) /'(' /(/s*) /({REGEX_VARIABLE}) /(/s* '<-')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var delimiter = tokens[2];
         Color(delimiter.Length, Structures);
         Color(tokens[3].Length, Whitespaces);
         var parameterName = tokens[4];
         Color(parameterName.Length, Variables);
         Color(tokens[5].Length, Structures);

         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var block, out var index))
         {
            overridePosition = index;
            var generator = new Generator(parameterName, block);
            result.Value = generator;
            return new Push(generator);
         }

         return null;
      }

      public override string VerboseName => "generator";
   }
}
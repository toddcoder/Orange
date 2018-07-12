using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class OperatorParser : Parser
   {
      FunctionBodyParser functionBodyParser;

      public OperatorParser()
         : base("^ /(|tabs|) /('nofix' | 'prefix' | 'suffix' | 'infix') /(/s+) /(['a-z'] ['a-z0-9']* '?'?) /(/s* '(')") =>
         functionBodyParser = new FunctionBodyParser();

      public override Verb CreateVerb(string[] tokens)
      {
         var type = tokens[2];
         var name = tokens[4];

         Color(position, tokens[1].Length, Whitespaces);
         Color(type.Length, KeyWords);
         Color(tokens[3].Length, Whitespaces);
         Color(name.Length, Operators);
         Color(tokens[5].Length, Structures);

         var index = position + length;
         var parametersParser = new ParametersParser();
         var parsed = parametersParser.Parse(source, index);
         Assert(parsed.If(out var parameters, out index), "Operator parser", "Parameters malformed");
         if (functionBodyParser.Parse(source, index).If(out var block, out var newIndex))
         {
            overridePosition = newIndex;
            var lambda = new Lambda(new Region(), block, parameters, false);
            var affinity = 0;
            var pre = false;
            switch (type)
            {
               case "nofix":
                  affinity = 0;
                  break;
               case "prefix":
                  affinity = 1;
                  pre = true;
                  break;
               case "postfix":
                  affinity = 1;
                  break;
               case "infix":
                  affinity = 2;
                  break;
            }

            var userDefinedOperator = new UserDefinedOperator(affinity, pre, lambda);
            CompilerState.RegisterOperator(name, userDefinedOperator);
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "operator";
   }
}
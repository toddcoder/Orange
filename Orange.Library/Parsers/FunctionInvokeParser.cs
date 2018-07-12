using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class FunctionInvokeParser : Parser
   {
      public FunctionInvokeParser()
         : base($"^ /(|sp|) /({REGEX_VARIABLE}) /('(') /(/s*)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var functionName = tokens[2];

         Color(position, tokens[1].Length, Whitespaces);
         Color(functionName.Length, IsClassName(functionName) ? Types : Invokeables);
         Color(tokens[3].Length, Structures);
         Color(tokens[4].Length, Whitespaces);

         if (GetExpression(source, NextPosition, CloseParenthesis()) is Some<(Block, int)> some)
         {
            (var block, var index) = some.Value;
            var arguments = new Arguments(block);
            var blockOrLambdaParser = new BlockOrLambdaParser();
            if (blockOrLambdaParser.Scan(source, index))
            {
               index = blockOrLambdaParser.Position;
               var value = blockOrLambdaParser.Value;
               arguments.AddArgument(value);
            }
            overridePosition = index;
            return new FunctionInvoke(functionName, arguments);
         }

         return null;
      }

      public override string VerboseName => "function invoke";
   }
}
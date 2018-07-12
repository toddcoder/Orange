using System.Collections.Generic;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class CurriedFunctionParser : Parser
   {
      string functionName;
      Parameters firstParameters;
      Object.VisibilityType visibility;
      bool overriding;
      ParametersParser parametersParser;
      FreeParser freeParser;
      FunctionBodyParser functionBodyParser;

      public CurriedFunctionParser(string functionName, Parameters firstParameters, Object.VisibilityType visibility, bool overriding)
         : base("^ /(|sp| '(')")
      {
         this.functionName = functionName;
         this.firstParameters = firstParameters;
         this.visibility = visibility;
         this.overriding = overriding;
         parametersParser = new ParametersParser();
         freeParser = new FreeParser();
         functionBodyParser = new FunctionBodyParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         var index = NextPosition;
         var stack = new Stack<Parameters>();
         stack.Push(firstParameters);
         while (index < source.Length)
            if (parametersParser.Parse(source, index).If(out var result1))
            {
               (var parameters, var i) = result1;
               index = i;
               stack.Push(parameters);
               if (freeParser.Scan(source, index, pattern))
               {
                  index = freeParser.Position;
                  freeParser.ColorAll(Structures);
                  continue;
               }

               break;
            }

         if (index < source.Length && functionBodyParser.Parse(source, index).If(out var result2))
         {
            (var block, var j) = result2;
            var lambda = none<Lambda>();
            while (stack.Count > 0)
            {
               var parameters = stack.Pop();
               lambda = lambda.FlatMap(l => getLambda(parameters, l), () => new Lambda(new Region(), block, parameters, false)).Some();
            }

            overridePosition = j;
            return new CreateFunction(functionName, lambda.Value, false, visibility, overriding, null) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "Curried function";

      static Lambda getLambda(Parameters parameters, Lambda lambda)
      {
         var builder = new CodeBuilder();
         builder.Parameters(parameters);
         builder.CreateLambda(lambda);
         return builder.Lambda();
      }
   }
}
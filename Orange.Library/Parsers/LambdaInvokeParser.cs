using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class LambdaInvokeParser : Parser
   {
      InvokeParser invokeParser;

      public LambdaInvokeParser()
         : base($"^ /(/s*) /({REGEX_VARIABLE}) '('") => invokeParser = new InvokeParser();

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var variableName = tokens[2];
         Color(variableName.Length, Variables);
         var index = position + length - 1;
         if (invokeParser.Scan(source, index))
         {
            var invoke = invokeParser.Result.Verb;
            overridePosition = invokeParser.Result.Position;
            var block = new Block
            {
               new Push(new Variable(variableName)),
               invoke
            };
            result.Value = block;
            return new NullOp();
         }
         return null;
      }

      public override string VerboseName => "Closure invoke";
   }
}
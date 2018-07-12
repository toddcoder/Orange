using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class MultilineFunctionInvokeParser : Parser
   {
      ExpressionLineParser parser;

      public MultilineFunctionInvokeParser()
         : base($"^ /(|sp|) /({REGEX_VARIABLE}) /':' /([/r /n]+)") => parser = new ExpressionLineParser();

      public override Verb CreateVerb(string[] tokens)
      {
         var invokeableName = tokens[2];
         Color(position, tokens[1].Length, Whitespaces);
         Color(invokeableName.Length, Invokeables);
         Color(tokens[3].Length, Structures);
         Color(tokens[4].Length, Whitespaces);

         AdvanceTabs();
         var index = NextPosition;
         var arguments = new Arguments();
         while (index < source.Length)
            if (parser.Scan(source, index))
            {
               index = parser.Position;
               var expression = (Block)parser.Value;
               arguments.AddBlockArgument(expression);
            }
            else
               break;

         RegressTabs();
         return new FunctionInvoke(invokeableName, arguments);
      }

      public override string VerboseName => "multiline function invoke";
   }
}
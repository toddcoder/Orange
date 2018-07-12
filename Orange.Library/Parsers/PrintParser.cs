using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class PrintParser : Parser
   {
      public PrintParser()
         : base("^ |tabs| /('println' /b | 'print' /b | 'put' /b | 'manifln' /b | 'manif' /b) |sp|") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var functionName = tokens[1];

         Color(position, length, KeyWords);

         if (GetExpression(source, NextPosition, EndOfLine()).If(out var exp, out var index))
         {
            overridePosition = index;
            Print.PrintType printType;
            switch (functionName)
            {
               case "println":
               case "print":
                  printType = Print.PrintType.Print;
                  break;
               case "manifln":
               case "manif":
                  printType = Print.PrintType.Manifest;
                  break;
               default:
                  printType = Print.PrintType.Put;
                  break;
            }

            return new Print(exp, printType, functionName.EndsWith("ln")) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "print";
   }
}
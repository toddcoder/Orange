using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class FunctionElementParser : Parser, IElementParser
   {
      public FunctionElementParser()
         : base($"^ /(/s*) /({REGEX_VARIABLE}) '('") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(tokens[1].Length, Whitespaces);
         var functionName = tokens[2];
         Color(functionName.Length, Variables);
         Color(1, Structures);
         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var block, out var index))
         {
            var arguments = new Arguments(block);
            Element = new FunctionElement(functionName, arguments);
            overridePosition = index;
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "function element";

      public Element Element { get; set; }
   }
}
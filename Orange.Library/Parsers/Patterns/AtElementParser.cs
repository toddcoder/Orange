using Orange.Library.Patterns;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Patterns
{
   public class AtElementParser : Parser, IElementParser
   {
      public AtElementParser()
         : base("^ /(/s* '@') /('(' | /d+)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Operators);
         var atSource = tokens[2];
         if (atSource == "(")
         {
            Color(atSource.Length, Structures);
            if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var at, out var index))
            {
               Element = new AtBlockElement(at);
               overridePosition = index;
               return new NullOp();
            }

            return null;
         }

         Color(atSource.Length, Numbers);
         var i = atSource.ToInt();
         Element = new AtElement(i);
         return new NullOp();
      }

      public override string VerboseName => "at element";

      public Element Element { get; set; }
   }
}
using Orange.Library.Scanning;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers.Scanning
{
   public class MoveParser : Parser, IScanItem
   {
      public MoveParser()
         : base("^ /(/s* '->(')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length - 1, Operators);
         Color(1, Structures);
         if (GetExpression(source, NextPosition, CloseParenthesis()).If(out var expression, out var index))
         {
            overridePosition = index;
            ScanItem = new Move(expression);
            return new NullOp();
         }

         return null;
      }

      public override string VerboseName => "move";

      public ScanItem ScanItem { get; set; }
   }
}
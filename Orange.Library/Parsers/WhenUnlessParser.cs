using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class WhenUnlessParser : Parser
   {
      public WhenUnlessParser()
         : base("^ /(' '*) /('when' | 'unless') /(' '*)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var type = tokens[2];
         Color(position, tokens[1].Length, Whitespaces);
         Color(type.Length, KeyWords);
         Color(tokens[3].Length, Whitespaces);

         if (GetExpression(source, NextPosition, EndOfLine()).If(out var expression, out var index))
         {
            overridePosition = index;
            return type == "when" ? new WhenVerb(OtherVerb, expression) : new UnlessVerb(OtherVerb, expression);
         }

         return OtherVerb;
      }

      public override string VerboseName => "when/unless verb";

      public Verb OtherVerb { get; set; }
   }
}
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class IndentParser : Parser
   {
      public IndentParser()
         : base("^ /(|tabs|) /('indent') /b /(/s+ ['-+'])?") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var type = tokens[2];
         var relative = tokens[3].Trim();
         var isRelative = relative.IsNotEmpty();

         Color(position, tokens[1].Length, Whitespaces);
         Color(type.Length, KeyWords);
         Color(tokens[3].Length, Operators);

         var matched = matchEndOfLine();
         switch (matched)
         {
            case EndOfLineType.EndOfSource:
            case EndOfLineType.EndOfLine:
               if (isRelative)
                  return null;

               return new Indent(((Values.Double)1).Pushed, false, 0) { Index = position };
            case EndOfLineType.More:
               if (GetExpression(source, NextPosition, EndOfLineConsuming()).If(out var e, out var i))
               {
                  overridePosition = i;
                  return new Indent(e, isRelative, relative == "+" ? 1 : -1) { Index = position };
               }

               break;
         }

         return null;
      }

      public override string VerboseName => "indent";
   }
}
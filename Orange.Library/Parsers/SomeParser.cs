using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;

namespace Orange.Library.Parsers
{
   public class SomeParser : Parser
   {
      public SomeParser()
         : base("^ /(' '*) /('?(')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         Color(tokens[2].Length, Structures);

         var index = position + length;
         var matches = source.Skip(index).Matches("^ /s* ')'");
         if (matches.IsSome)
         {
            var matcher = matches.Value;
            Color(matcher.Length, Structures);
            overridePosition = index + matcher.Length;
            result.Value = new None();
            return new PushNone();
         }

         if (GetExpression(source, index, CloseBracket()).If(out var block, out index))
         {
            overridePosition = index;
            return new PushSome(block);
         }

         return null;
      }

      public override string VerboseName => "some";
   }
}
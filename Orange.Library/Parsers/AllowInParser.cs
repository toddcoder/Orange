using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.OrangeCompiler;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class AllowInParser : Parser
   {
      public AllowInParser()
         : base("^ /(/s* 'allow') /(/s* '(')")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         Color(tokens[2].Length, Structures);
         var index = position + length;
         Block definition;
         ParseBlock(source, index, "')'").Assign(out definition, out index);
         var matches = source.Skip(index).Matches("^ /(/s* 'in') /(/s* '(')");
         if (matches.IsSome)
         {
            var evaluation = new Block();
            matches.Do(matcher =>
            {
               Color(matcher[0, 1].Length, KeyWords);
               Color(matcher[0, 2].Length, Structures);
               index += matcher[0].Length;
               ParseBlock(source, index, "')'").Assign(out evaluation, out index);
               overridePosition = index;
            });
            return new AllowIn(definition, evaluation);
         }
         return null;
      }

      public override string VerboseName => "allow in";
   }
}
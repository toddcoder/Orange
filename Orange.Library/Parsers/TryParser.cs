using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library.Parsers
{
   public class TryParser : Parser
   {
      public TryParser()
         : base($"^ /(|tabs| 'try') (/s+ /({REGEX_VARIABLE}) /s* /'=' /s*) /(/r /n | /r | /n)") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var fieldName = tokens[2];
         Color(position, tokens[1].Length, KeyWords);
         Color(fieldName.Length, Variables);
         Color(tokens[3].Length, Structures);
         Color(tokens[4].Length, Whitespaces);

         if (GetBlock(source, NextPosition, true).If(out var block, out var index))
         {
            overridePosition = index;
            return new Try(fieldName, block);
         }

         return null;
      }

      public override string VerboseName => "try";
   }
}
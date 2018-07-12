using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class SymbolParser : Parser
   {
      public SymbolParser()
         : base($"^ |sp| '%' /({REGEX_VARIABLE})")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Symbols);
         result.Value = new Symbol(tokens[1]);
         return new Push(result.Value);
      }

      public override string VerboseName => "symbol";
   }
}
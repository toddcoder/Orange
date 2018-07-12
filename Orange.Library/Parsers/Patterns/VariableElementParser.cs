using Orange.Library.Patterns;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Patterns
{
   public class VariableElementParser : Parser, IElementParser
   {
      public VariableElementParser()
         : base($"^ /(/s*) /'~'? /({REGEX_VARIABLE})", true)
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var breakSymbol = tokens[2];
         var variableName = tokens[3].ToLower();
         Color(position, tokens[1].Length, Whitespaces);
         Color(breakSymbol.Length, Operators);
         Color(variableName.Length, Variables);

         Element = breakSymbol == "~" ? (Element)new BreakVariableElement(variableName) : new VariableElement(variableName);

         return new NullOp();
      }

      public override string VerboseName => "variable element";

      public Element Element
      {
         get;
         set;
      }
   }
}
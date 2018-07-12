using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.AnyBlockParser;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class CaseParser : Parser
   {
      AnyBlockParser anyBlockParser;

      public CaseParser()
         : base("^ /(/s* 'case' | 'required') /b")
      {
         anyBlockParser = new AnyBlockParser(REGEX_STANDARD_BRIDGE);
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         var index = position + length;

         Block condition;
         var comparisand = ComparisandParser.Parse(source, ref index, out condition, @"^ /s* [':{']");
         if (source.Substring(index - 1, 1) == ":")
            index--;
         var block = anyBlockParser.Parse(source, ref index, false);
         if (block == null)
            return null;
         overridePosition = index;
         return new CaseExecute(comparisand, block, tokens[1].EndsWith("required"), condition);
      }

      public override string VerboseName => "case";
   }
}
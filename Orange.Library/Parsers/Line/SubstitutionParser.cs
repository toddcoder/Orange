using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers.Line
{
   public class SubstitutionParser : Parser
   {
      FillInValueParser parser;

      public SubstitutionParser(FillInValueParser parser)
         : base("^ ' '* '$' /(/d+)") => this.parser = parser;

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Variables);
         var number = tokens[1];
         var name = MangledName(number);
         parser.Variable = name;
         var variable = new Variable(name);
         result.Value = variable;
         return new Push(variable);
      }

      public override string VerboseName => "substitution";
   }
}
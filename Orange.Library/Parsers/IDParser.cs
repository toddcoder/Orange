using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Compiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class IDParser : Parser
   {
      public IDParser()
         : base($"^ /(/s*) /({REGEX_VARIABLE}) /('*')") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Whitespaces);
         var variableName = tokens[2];
         Color(variableName.Length, Variables);
         Color(tokens[3].Length, Operators);
         variableName += "-" + CompilerState.ObjectID();
         return new Push(new Variable(variableName));
      }

      public override string VerboseName => "id";
   }
}
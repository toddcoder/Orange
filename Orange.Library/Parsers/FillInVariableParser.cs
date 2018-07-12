using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class FillInVariableParser : Parser
   {
      public static int Index { get; set; }

      public FillInVariableParser()
         : base("^ /(/s*) /('_' | '$' /d+) /b") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Variables);
         var name = tokens[2];
         name = name == "_" ? MangledName(Index++.ToString()) : $"__{name}";
         var variable = new Variable(name);
         result.Value = variable;
         return new Push(variable);
      }

      public override string VerboseName => "fill in value";
   }
}
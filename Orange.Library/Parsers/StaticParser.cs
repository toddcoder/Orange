using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Compiler;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class StaticParser : Parser
   {
      const string LOCATION = "Static parser";

      public StaticParser()
         : base($"^ /(|tabs| 'meta' /b) /(' '+ {REGEX_VARIABLE})?") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var className = tokens[2].Trim();
         Color(position, tokens[1].Length, KeyWords);
         Color(tokens[2].Length, Variables);
         if (GetBlock(source, NextPosition, true).If(out var block, out var index))
         {
            overridePosition = index;
            if (className == "")
               className = CompilerState.LastClassName;
            return new CreateStaticBlock(className, block);
         }

         return null;
      }

      public override string VerboseName => "static block";
   }
}
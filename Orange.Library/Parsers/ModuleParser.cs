using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Parsers.StatementParser;

namespace Orange.Library.Parsers
{
   public class ModuleParser : Parser
   {
      public ModuleParser()
         : base($"^ /(|tabs| 'module' /s+) /({REGEX_VARIABLE})") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var moduleName = tokens[2];
         Color(position, tokens[1].Length, KeyWords);
         Color(moduleName.Length, Variables);

         var endLineParser = new EndOfLineParser();
         endLineParser.Scan(source, NextPosition);
         var index = endLineParser.Position;

         if (GetBlock(source, index, true).If(out var block, out var i))
         {
            overridePosition = i;
            return new CreateNewModule(moduleName, new NewModule(block));
         }

         return null;
      }

      public override string VerboseName => "module";
   }
}
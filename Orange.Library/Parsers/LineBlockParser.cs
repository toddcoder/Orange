using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class LineBlockParser : Parser
   {
      public LineBlockParser()
         : base($"^ |sp| /'do' {REGEX_END}") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);

         AdvanceTabs();

         var statementParser = new StatementParser();
         var block = new Block();

         var index = NextPosition;
         while (index < source.Length)
            if (statementParser.Scan(source, index))
            {
               index = statementParser.Position;
               var verb = statementParser.Verb;
               if (verb != null && !(verb is NullOp))
                  block.Add(verb);
            }
            else
               break;

         RegressTabs();

         overridePosition = index;
         Block = block;
         return block.PushedVerb;
      }

      public override string VerboseName => "line block";

      public Block Block { get; set; }
   }
}
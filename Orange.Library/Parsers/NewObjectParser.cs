using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.ClassParser;
using static Orange.Library.Parsers.StatementParser;
using static Orange.Library.Parsers.StatementParser.InclusionType;

namespace Orange.Library.Parsers
{
   public class NewObjectParser : Parser
   {
      ParametersParser parametersParser;

      public NewObjectParser()
         : base("^ ' '* '.('") => parametersParser = new ParametersParser();

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         if (parametersParser.Parse(source, NextPosition).If(out var parameters, out var index))
         {
            (var _, var _, var _, var newIndex) = Ancestors(source, index);
            index = newIndex;
            InClassDefinition = true;
            var objectBlock = new Block();
            GetBlock(source, index, true, InClass).If(out objectBlock, out index);
            InClassDefinition = false;
            var cls = new Class(parameters, objectBlock);
            overridePosition = index;
            result.Value = cls;
            return new CreateModule("", cls, false) { Index = position };
         }

         return null;
      }

      public override string VerboseName => "new object";
   }
}
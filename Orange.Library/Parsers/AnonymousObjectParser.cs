using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using static Orange.Library.Compiler;
using static Orange.Library.OrangeCompiler;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Object.VisibilityType;

namespace Orange.Library.Parsers
{
   public class AnonymousObjectParser : Parser
   {
      InheritanceParser inheritanceParser;
      DoesParser doesParser;

      public AnonymousObjectParser()
         : base("^ /(/s* '!') '{'")
      {
         inheritanceParser = new InheritanceParser();
         doesParser = new DoesParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         Color(1, Structures);
         var index = position + length;
         var superName = "";
         Parameters superParameters = null;
         if (inheritanceParser.Scan(source, index))
         {
            superName = inheritanceParser.VariableName;
            superParameters = inheritanceParser.Parameters;
            index = inheritanceParser.Result.Position;
         }
         var traits = new List<string>();
         if (doesParser.Scan(source, index))
         {
            traits = doesParser.Traits;
            index = doesParser.Result.Position;
         }

         Block objectBlock;
         try
         {
            InClassDefinition = true;
            ParseBlock(source, index, "'}'").Assign(out objectBlock, out index);
         }
         finally
         {
            InClassDefinition = false;
         }

         var builder = new Class(new Parameters(), objectBlock, GetStaticBlock(), superName, traits.ToArray(),
            superParameters, false);
         result.Value = builder;
         overridePosition = index;
         return new CreateObject(VAR_ANONYMOUS + CompilerState.ObjectID(), builder, false, Protected);
      }

      public override string VerboseName => "anonymous object";
   }
}
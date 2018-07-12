using System.Collections.Generic;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using static Orange.Library.Parsers.ClassParser;
using static Orange.Library.Parsers.IDEColor;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class UnionParser : Parser
   {
      InheritanceParser inheritanceParser;
      DoesParser doesParser;

      public UnionParser()
         : base($"^ /(/s*) /('union') /(/s*) /({REGEX_VARIABLE})")
      {
         inheritanceParser = new InheritanceParser();
         doesParser = new DoesParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         if (InClassDefinition)
            return null;

         Color(position, tokens[1].Length, EntityType.Whitespaces);
         Color(tokens[2].Length, EntityType.KeyWords);
         Color(tokens[3].Length, EntityType.Whitespaces);
         var unionName = ClassName = tokens[4];
         Color(unionName.Length, EntityType.Variables);

         var index = position + length;

         string superClass;
         Parameters superParameters;
         string[] traits;
         Ancestors(source, index).Assign(out superClass, out superParameters, out traits, out index);

         var builder = new CodeBuilder();

         return null;
      }

      public override string VerboseName => "Union";
   }
}
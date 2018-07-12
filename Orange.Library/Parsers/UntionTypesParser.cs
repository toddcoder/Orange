using System.Linq;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using static Orange.Library.Parsers.IDEColor.EntityType;
using Standard.Types.Strings;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class UntionTypesParser : Parser
   {
      public UntionTypesParser()
         : base($"^ /(/s*) /'types'")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, Structures);
         Color(tokens[2].Length, Variables);

         var parametersParser = new ParametersParser();
         var index = position + length;
         var matcher = new Matcher();
         var current = source.Skip(index);

         while (!matcher.IsMatch(current, "^ /(/s*) /'}'"))
         {
            if (matcher.IsMatch(current, $"^ /(/s*) /({REGEX_VARIABLE})"))
            {
               var groups = matcher.Groups(0);
               Color(index, groups[1].Length, Whitespaces);
               var typeName = groups[2];
               Color(typeName.Length, Variables);
               Parameters parameters = null;
               current = current.Skip(matcher.Length);
               if (matcher.IsMatch(current, "^ /s* '('"))
               {
                  Color(index,Structures);
                  index += matcher.Length;
                  current = current.Skip(matcher.Length);
                  if (parametersParser.Parse(current, ref index))
                  {
                     current = source.Skip(index);
                     parameters = parametersParser.Parameters;
                  }
               }
            }
         }

         return null;
      }

      void addType(string typeName, Parameters parameters)
      {

      }

      public override string VerboseName => "untion types";
   }
}
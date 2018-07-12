using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class GrammarParser : Parser
   {
      Matcher matcher;
      PatternParser patternParser;

      public GrammarParser()
         : base($"^ /(/s* 'grammar') /(/s* {REGEX_VARIABLE}) /(/s* '{{')")
      {
         matcher = new Matcher();
         patternParser = new PatternParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, tokens[1].Length, KeyWords);
         var grammarName = tokens[2].Trim();
         Color(grammarName.Length, Variables);
         Color(tokens[3].Length, Structures);

         var index = position + length;
         var patterns = new Hash<string, Pattern>();
         var firstRule = "";
         var sourceLength = source.Length;

         while (index < sourceLength)
         {
            var input = source.Substring(index);
            if (matcher.IsMatch(input, "^ /s* '}'"))
            {
               var matchLength = matcher[0].Length;
               Color(matchLength, Structures);
               overridePosition = matchLength + index;
               return new CreateGrammar(grammarName, patterns, firstRule);
            }

            if (matcher.IsMatch(input, $"^ /(/s*) /({REGEX_VARIABLE}) /(/s* '<-' /s*)"))
            {
               Color(matcher[0, 1].Length, Whitespaces);
               var ruleName = matcher[0, 2];
               if (firstRule.IsEmpty())
                  firstRule = ruleName;
               Color(ruleName.Length, Variables);
               Color(matcher[0, 3].Length, Structures);
               index += matcher[0].Length;
               if (patternParser.Scan(source, index))
               {
                  var parserResult = patternParser.Result;
                  var parsedPattern = (Pattern)parserResult.Value;
                  index = parserResult.Position;
                  patterns[ruleName] = parsedPattern;
               }
               else
                  return null;
            }
            else
               return null;
         }

         return null;
      }

      public override string VerboseName => "grammar";
   }
}
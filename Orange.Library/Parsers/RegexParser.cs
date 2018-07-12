using System.Text;
using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class RegexParser : Parser
   {
      enum StatusType
      {
         Outside,
         WaitingForSingleQuote,
         WaitingForDoubleQuote,
         EscapedSingleQuote,
         EscapedDoubleQuote,
         AwaitingOption,
         VariableBeginning,
         Variable
      }

      public RegexParser()
         : base(@"^ ' '* '\'") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);

         var regex = new StringBuilder();
         var type = StatusType.Outside;
         var ignoreCaseOption = false;
         var multilineOption = false;
         var globalOption = false;

         for (var i = position + length; i < source.Length; i++)
         {
            var ch = source[i];
            switch (type)
            {
               case StatusType.Outside:
                  switch (ch)
                  {
                     case '\\':
                        var rxPatern = regex.ToString();
                        result.Value = new Regex(rxPatern, ignoreCaseOption, multilineOption, globalOption);
                        Color(1, Structures);
                        overridePosition = i + 1;
                        return new Push(result.Value);
                     case '\'':
                        type = StatusType.WaitingForSingleQuote;
                        regex.Append(ch);
                        Color(1, Strings);
                        break;
                     case '"':
                        type = StatusType.WaitingForDoubleQuote;
                        Color(1, Strings);
                        regex.Append(ch);
                        break;
                     case ';':
                        type = StatusType.AwaitingOption;
                        Color(1, Symbols);
                        break;
                     case '%':
                        type = StatusType.VariableBeginning;
                        Color(1, Structures);
                        regex.Append(ch);
                        break;
                     default:
                        regex.Append(ch);
                        var color = Operators;
                        switch (ch)
                        {
                           case '{':
                           case '}':
                           case '>':
                           case '<':
                           case '[':
                           case ']':
                           case ',':
                           case '|':
                           case '(':
                           case ')':
                              color = Structures;
                              break;
                           default:
                              if (char.IsNumber(ch))
                                 color = Numbers;
                              else if (char.IsLetter(ch))
                                 color = Variables;
                              break;
                        }

                        Color(1, color);
                        break;
                  }

                  break;
               case StatusType.WaitingForSingleQuote:
                  switch (ch)
                  {
                     case '\'':
                        type = StatusType.Outside;
                        regex.Append(ch);
                        break;
                     case '`':
                        type = StatusType.EscapedSingleQuote;
                        regex.Append(ch);
                        break;
                     default:
                        regex.Append(ch);
                        break;
                  }

                  Color(1, Strings);
                  break;
               case StatusType.WaitingForDoubleQuote:
                  switch (ch)
                  {
                     case '"':
                        type = StatusType.Outside;
                        regex.Append(ch);
                        break;
                     case '`':
                        type = StatusType.EscapedDoubleQuote;
                        regex.Append(ch);
                        break;
                     default:
                        regex.Append(ch);
                        break;
                  }

                  Color(1, Strings);
                  break;
               case StatusType.EscapedSingleQuote:
                  type = StatusType.WaitingForSingleQuote;
                  regex.Append(ch);
                  Color(1, Strings);
                  break;
               case StatusType.EscapedDoubleQuote:
                  type = StatusType.WaitingForDoubleQuote;
                  regex.Append(ch);
                  Color(1, Strings);
                  break;
               case StatusType.AwaitingOption:
                  switch (ch)
                  {
                     case 'i':
                     case 'I':
                        ignoreCaseOption = true;
                        break;
                     case 'm':
                     case 'M':
                        multilineOption = true;
                        break;
                     case 'g':
                     case 'G':
                        globalOption = true;
                        break;
                     default:
                        type = StatusType.Outside;
                        i--;
                        continue;
                  }

                  type = StatusType.Outside;
                  Color(1, Symbols);
                  break;
               case StatusType.VariableBeginning:
                  switch (ch)
                  {
                     case '(':
                        type = StatusType.Variable;
                        Color(1, Structures);
                        regex.Append(ch);
                        break;
                  }

                  type = StatusType.Outside;
                  goto case StatusType.Outside;
               case StatusType.Variable:
                  switch (ch)
                  {
                     case ')':
                        Color(1, Structures);
                        type = StatusType.Outside;
                        break;
                     default:
                        Color(1, Variables);
                        break;
                  }

                  regex.Append(ch);
                  break;
            }
         }

         return null;
      }

      public override string VerboseName => "Regex";
   }
}
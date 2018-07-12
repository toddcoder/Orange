using System.Collections.Generic;
using System.Text;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using Standard.Types.Tuples;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class InterpolatedStringParser : Parser
   {
      enum Stage
      {
         String,
         Variable1,
         Variable,
         Message1,
         Message,
         Format1,
         Format
      }

      static bool isVariable1(char c) => c.ToString().IsMatch("['$a-zA-Z_']");

      static bool isVariable(char c) => c.ToString().IsMatch("['a-zA-Z_0-9']");

      static bool isFormat1(char c) => c.ToString().IsMatch("['cdefgnprxs']");

      static bool isFormat(char c) => c.ToString().IsMatch("['0-9.-']");

      public InterpolatedStringParser()
         : base("^ /(|sp|) /(['$#']) /([quote '[<'])") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Interpolated);
         var type = tokens[2];
         var quote = tokens[3][0];
         switch (quote)
         {
            case '[':
               quote = ']';
               break;
            case '<':
               quote = '>';
               break;
         }

         var escaped = false;
         var text = new StringBuilder();
         var blocks = new List<Block>();
         var start = position + length;
         var stage = Stage.String;

         for (var i = start; i < source.Length; i++)
         {
            var c = source[i];
            switch (c)
            {
               case '`':
                  if (escaped)
                  {
                     text.Append("`");
                     escaped = false;
                  }
                  else
                     escaped = true;
                  Color(1, Interpolated);
                  break;
               /*					case '#':
                                 if (escaped)
                                 {
                                    text.Append("#");
                                    escaped = false;
                                    Color(1, Strings);
                                 }
                                 else
                                 {
                                    text.Append("`#");
                                    Color(1, Strings);
                                    stage = Stage.Variable1;
                                 }
                                 break;*/
               case '(':
                  if (escaped)
                  {
                     text.Append("(");
                     escaped = false;
                     Color(1, Interpolated);
                  }
                  else
                  {
                     Color(1, Structures);
                     var index = i + 1;
                     Block block;
                     if (GetExpression(source, index, CloseParenthesis()).Assign(out block, out index))
                     {
                        text.Append($"#{blocks.Count}#");
                        blocks.Add(block);
                        i = index - 1;
                     }
                     else
                        return null;
                  }

                  break;
               default:
                  if (c == quote)
                  {
                     if (escaped)
                     {
                        text.Append(quote);
                        escaped = false;
                        Color(1, Interpolated);
                        continue;
                     }

                     overridePosition = i + 1;
                     switch (stage)
                     {
                        case Stage.Variable1:
                        case Stage.Message1:
                        case Stage.Format1:
                           Color(i - 1, 2, Interpolated);
                           break;
                        default:
                           Color(1, Interpolated);
                           break;
                     }

                     var newText = ReplaceEscapedValues(text.ToString());
                     newText = replaceVariableCalls(newText, blocks);
                     var matcher = new Matcher();
                     if (matcher.IsMatch(newText, "-(< '`') '|' /(['0-9a-fA-f']+) '|'"))
                     {
                        for (var j = 0; j < matcher.MatchCount; j++)
                           matcher[j, 0] = getHex(matcher[j, 1]).ToString();

                        newText = matcher.ToString();
                     }

                     var interpolatedString = new InterpolatedString(newText.Substitute("^ /(/r/n | /r | /n)", ""), blocks);
                     result.Value = type == "$" ? (Value)interpolatedString : new Failure(interpolatedString);
                     return new Push(result.Value);
                  }

                  if (escaped)
                  {
                     switch (c)
                     {
                        case 't':
                           text.Append('\t');
                           Color(1, Interpolated);
                           break;
                        case 'r':
                           text.Append('\r');
                           Color(1, Interpolated);
                           break;
                        case 'n':
                           text.Append('\n');
                           Color(1, Interpolated);
                           break;
                        case 'l':
                           text.Append("\r\n");
                           Color(2, Interpolated);
                           break;
                        default:
                           text.Append("`");
                           text.Append(c);
                           Color(2, Interpolated);
                           break;
                     }

                     escaped = false;
                  }
                  else
                  {
                     text.Append(c);
                     switch (stage)
                     {
                        case Stage.String:
                           switch (c)
                           {
                              case '\\':
                                 Color(1, Operators);
                                 stage = Stage.Format1;
                                 break;
                              case '.':
                                 Color(1, Structures);
                                 stage = Stage.Message1;
                                 break;
                              default:
                                 Color(1, Interpolated);
                                 break;
                           }

                           break;
                        case Stage.Variable1:
                           if (isVariable1(c))
                           {
                              Color(1, Variables);
                              stage = Stage.Variable;
                           }
                           else if (c == '@' || c == '.')
                           {
                              Color(1, Structures);
                              stage = Stage.Message1;
                           }
                           else
                           {
                              Color(i - 1, 2, Interpolated);
                              stage = Stage.String;
                           }
                           break;
                        case Stage.Variable:
                           if (isVariable(c))
                              Color(1, Variables);
                           else
                              switch (c)
                              {
                                 case '\\':
                                    Color(1, Operators);
                                    stage = Stage.Format1;
                                    break;
                                 case '.':
                                    Color(1, Structures);
                                    stage = Stage.Message1;
                                    break;
                                 default:
                                    Color(1, Interpolated);
                                    stage = Stage.String;
                                    break;
                              }

                           break;
                        case Stage.Message1:
                           if (isVariable1(c))
                           {
                              Color(1, Messaging);
                              stage = Stage.Message;
                           }
                           else
                           {
                              Color(i - 1, 2, Interpolated);
                              stage = Stage.String;
                           }

                           break;
                        case Stage.Message:
                           if (isVariable(c))
                              Color(1, Messaging);
                           else
                              switch (c)
                              {
                                 case '\\':
                                    Color(1, Operators);
                                    stage = Stage.Format1;
                                    break;
                                 default:
                                    Color(1, Interpolated);
                                    stage = Stage.String;
                                    break;
                              }

                           break;
                        case Stage.Format1:
                           if (char.IsWhiteSpace(c))
                              Color(1, Interpolated);
                           else if (isFormat1(c))
                           {
                              Color(i - 1, 2, Formats);
                              stage = Stage.Format;
                           }
                           else
                           {
                              Color(1, Interpolated);
                              stage = Stage.String;
                           }
                           break;
                        case Stage.Format:
                           if (isFormat(c))
                              Color(1, Formats);
                           else
                           {
                              Color(1, Interpolated);
                              stage = Stage.String;
                           }
                           break;
                     }
                  }

                  break;
            }
         }

         return null;
      }

      static char getHex(string unicode) => (char)HexParser.GetNumber(unicode);

      static string replaceVariableCalls(string source, List<Block> blocks)
      {
         var matcher = new Matcher();
         if (matcher.IsMatch(source, $"'`#' /(['@.'])? /({REGEX_VARIABLE}) /('.')? /({REGEX_VARIABLE})? " +
            "(/s* '\\' /s* /( /(['cdefgnprxs']) /('-'? /d+)? ('.' /(/d+))?))?", true))
            for (var i = 0; i < matcher.MatchCount; i++)
            {
               var self = matcher[i, 1];
               var variable = matcher[i, 2];
               var op = matcher[i, 3];
               var message = matcher[i, 4];
               var format = matcher[i, 5];
               var builder = new CodeBuilder();
               if (message.IsNotEmpty())
               {
                  builder.Variable(variable);
                  switch (op)
                  {
                     case ".":
                        builder.SendMessage(message, new Arguments(), registerCall: true);
                        break;
                  }
               }
               else
                  switch (self)
                  {
                     case ".":
                        builder.SendMessageToSelf(variable);
                        break;
                     case "@":
                        builder.SendMessageToClass(variable);
                        break;
                     default:
                        builder.Variable(variable);
                        break;
                  }

               if (format.IsNotEmpty())
               {
                  builder.Format();
                  builder.Value(format);
               }
               matcher[i] = "#" + blocks.Count + "#";
               blocks.Add(builder.Block);
            }

         return matcher.ToString();
      }

      public override string VerboseName => "interpolated string";
   }
}
using System.Collections.Generic;
using System.Text;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Runtime;

namespace Orange.Library.Parsers
{
   public class InterpolatedStringParser2 : Parser
   {
      public InterpolatedStringParser2()
         : base("^ |sp| /['$#'] /[quote]") { }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Interpolated);
         var type = tokens[1];
         var quote = tokens[2][0];
         var escaped = false;
         var text = new StringBuilder();
         var blocks = new List<Block>();
         for (var i = NextPosition; i < source.Length; i++)
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
                     if (GetExpression(source, index, CloseParenthesis()).If(out var block, out index))
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
                     Color(1, Interpolated);
                     var newText = ReplaceEscapedValues(text.ToString());
                     var interpolatedString = new InterpolatedString(newText.Substitute("^ /(/r/n | /r | /n)", ""),
                        blocks);
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
                           Color(1, Interpolated);
                           break;
                        default:
                           text.Append("`");
                           text.Append(c);
                           Color(1, Interpolated);
                           break;
                     }

                     escaped = false;
                  }
                  else
                  {
                     text.Append(c);
                     Color(1, Interpolated);
                  }

                  break;
            }
         }

         return null;
      }

      public override string VerboseName => "interpolated string";
   }
}
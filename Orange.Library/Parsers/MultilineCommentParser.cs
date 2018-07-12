using Orange.Library.Verbs;
using Standard.Types.Exceptions;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class MultilineCommentParser : Parser
   {
      enum ParseStateType
      {
         Outside,
         SingleString,
         DoubleString,
         AnticipatingInterploatedString,
         DoubleInterpolatedString,
         SingleInterpolatedString,
         Comment,
         AnticipatingClose,
         AnticipatingComment
      }

      public MultilineCommentParser()
         : base("^ /s* '//*'") { }

      public override Verb CreateVerb(string[] tokens)
      {
         var parseState = ParseStateType.Outside;
         var escape = false;
         for (var i = NextPosition; i < source.Length; i++)
         {
            var chr = source[i];
            switch (parseState)
            {
               case ParseStateType.Outside:
                  switch (chr)
                  {
                     case '\'':
                        parseState = ParseStateType.SingleString;
                        break;
                     case '"':
                        parseState = ParseStateType.DoubleString;
                        break;
                     case '$':
                        parseState = ParseStateType.AnticipatingInterploatedString;
                        break;
                     case '/':
                        parseState = ParseStateType.AnticipatingComment;
                        break;
                     case '*':
                        parseState = ParseStateType.AnticipatingClose;
                        break;
                  }

                  break;
               case ParseStateType.SingleString:
                  switch (chr)
                  {
                     case '\'':
                        if (escape)
                           escape = false;
                        else
                           parseState = ParseStateType.Outside;
                        break;
                     case '`':
                        escape = true;
                        break;
                     default:
                        escape = false;
                        break;
                  }

                  break;
               case ParseStateType.DoubleString:
                  switch (chr)
                  {
                     case '"':
                        if (escape)
                           escape = false;
                        else
                           parseState = ParseStateType.Outside;
                        break;
                     case '`':
                        escape = true;
                        break;
                     default:
                        escape = false;
                        break;
                  }

                  break;
               case ParseStateType.AnticipatingInterploatedString:
                  switch (chr)
                  {
                     case '"':
                        if (escape)
                        {
                           escape = false;
                           parseState = ParseStateType.Outside;
                        }
                        else
                           parseState = ParseStateType.DoubleInterpolatedString;
                        break;
                     case '\'':
                        if (escape)
                        {
                           escape = false;
                           parseState = ParseStateType.Outside;
                        }
                        else
                           parseState = ParseStateType.SingleInterpolatedString;
                        break;
                     default:
                        parseState = ParseStateType.Outside;
                        escape = false;
                        break;
                  }

                  break;
               case ParseStateType.SingleInterpolatedString:
                  switch (chr)
                  {
                     case '\'':
                        if (escape)
                           escape = false;
                        else
                           parseState = ParseStateType.Outside;
                        break;
                     case '`':
                        escape = true;
                        break;
                     default:
                        escape = false;
                        break;
                  }

                  break;
               case ParseStateType.DoubleInterpolatedString:
                  switch (chr)
                  {
                     case '"':
                        if (escape)
                           escape = false;
                        else
                           parseState = ParseStateType.Outside;
                        break;
                     case '`':
                        escape = true;
                        break;
                     default:
                        escape = false;
                        break;
                  }

                  break;
               case ParseStateType.Comment:
                  switch (chr)
                  {
                     case '\r':
                     case '\n':
                        parseState = ParseStateType.Outside;
                        break;
                  }

                  break;
               case ParseStateType.AnticipatingClose:
                  switch (chr)
                  {
                     case '/':
                        Color(position, i - position + 1, Comments);
                        overridePosition = i + 1;
                        return new NullOp();
                     default:
                        parseState = ParseStateType.Outside;
                        break;
                  }

                  break;
               case ParseStateType.AnticipatingComment:
                  switch (chr)
                  {
                     case '/':
                        parseState = ParseStateType.Comment;
                        break;
                     default:
                        parseState = ParseStateType.Outside;
                        break;
                  }

                  break;
            }
         }

         throw "Open comment".Throws();
      }

      public override string VerboseName => "Multiline comment";
   }
}
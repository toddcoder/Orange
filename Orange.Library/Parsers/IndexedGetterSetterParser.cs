using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Runtime;
using Standard.Types.Tuples;
using static System.Activator;
using static Orange.Library.Parsers.ExpressionParser;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Parsers.TwoCharacterOperatorParser;

namespace Orange.Library.Parsers
{
   public class IndexedGetterSetterParser : Parser
   {
      public static IMaybe<Block> CombineOperation(string message, string op, Block index, Block expression)
      {
         var type = Operator(op);
         if (type == null)
            return new None<Block>();
         var verb = (Verb)CreateInstance(type);
         var builder = new CodeBuilder();
         builder.SendMessage(GetterName(message), new Arguments(index));
         builder.Verb(verb);
         builder.Parenthesize(expression);
         return builder.Block.Some();
      }

      FreeParser freeParser;

      public IndexedGetterSetterParser()
         : base($"^ /(' '* '.')? /({REGEX_VARIABLE}) '['")
      {
         freeParser = new FreeParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         var hasPoint = tokens[1].Trim().IsNotEmpty();
         var message = tokens[2];
         Color(position, tokens[1].Length, Structures);
         Color(message.Length, Messaging);
         Color(1, Structures);

         return GetExpression(source, NextPosition, CloseBracket(), true).Map((argumentExp, i) =>
         {
            var index = i;
            var fieldName = "";
            if (freeParser.Scan(source, index, $"^ {REGEX_ASSIGN}"))
            {
               index = freeParser.Position;
               freeParser.Colorize(Whitespaces, Structures, Structures);
               var op = freeParser.Tokens[2];
               return GetExpression(source, index, EndOfLine()).Map((expression, j) =>
               {
                  overridePosition = j;
                  if (!hasPoint)
                  {
                     fieldName = message;
                     message = "item";
                  }
                  if (op.IsNotEmpty())
                  {
                     var combined = CombineOperation(message, op, argumentExp, expression);
                     if (combined.IsNone)
                        return null;
                     expression = combined.Value;
                  }
                  var arguments = new Arguments();
                  arguments.AddBlockArgument(argumentExp);
                  arguments.AddBlockArgument(expression);
                  return hasPoint ? new SendMessage(SetterName(message), arguments) :
                     new SendMessageToField(fieldName, SetterName(message), arguments);
               }, () => null);
            }
            overridePosition = index;
            return new SendMessage(GetterName(message), new Arguments(argumentExp));
         }, () => null);
      }

      public override string VerboseName => "indexed getter settter";
   }
}
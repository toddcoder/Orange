using System.Collections.Generic;
using Orange.Library.Parsers.Line;
using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;
using static Orange.Library.Parsers.Stop;
using static Orange.Library.Parsers.WordOperatorParser;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Parsers
{
   public class ShortLambdaParser : Parser
   {
      public static IMaybe<(Block, int)> GetExpression(string source, int index, Stop stop)
      {
         var parser = new ShortLambdaParser("", stop);
         return when(parser.Scan(source, index), () => (parser.Lambda.Block, parser.Position));
      }

      Stop stop;
      FillInValueParser valueParser;
      InfixOperatorParser operatorParser;
      SendMessageParser sendMessageParser;
      FreeParser freeParser;
      int maxIndex;

      public ShortLambdaParser(string prefix, Stop stop = null)
         : base($"^ /(' '*) /'{prefix}'")
      {
         valueParser = new FillInValueParser();
         operatorParser = new InfixOperatorParser();
         sendMessageParser = new SendMessageParser(false);
         freeParser = new FreeParser();
         this.stop = stop ?? CloseParenthesis();
      }

      IMaybe<int> getTerm(Block block, int index)
      {
         if (valueParser.Scan(source, index) && isNotWordOperator(valueParser.Value))
         {
            block.Add(valueParser.Verb);
            index = valueParser.Position;
            addPossibleParameter(valueParser.Value);
         }
         else
         {
            var name = MangledName($"{FillInVariableParser.Index++}");
            var variable = new Variable(name);
            var push = new Push(variable);
            block.Add(push);
            addPossibleParameter(variable);
         }
         while (sendMessageParser.Scan(source, index))
         {
            block.Add(sendMessageParser.Verb);
            index = sendMessageParser.Position;
         }

         return index.Some();
      }

      static bool isNotWordOperator(Value value) => !(value is Variable v) || !IsWordOperator(v.Name);

      void addPossibleParameter(Value value)
      {
         if (value is Variable variable)
            addPossibleParameter(variable.Name);
      }

      void addPossibleParameter(string name)
      {
         if (name.Matches("^ '__$' /(/d+)").If(out var matcher))
         {
            var index = matcher.FirstGroup.ToInt();
            if (index > maxIndex)
               maxIndex = index;
         }
      }

      Verb returnLambda(Block block, int index)
      {
         var list = new List<Parameter>();
         for (var i = 0; i <= maxIndex; i++)
         {
            var name = MangledName(i.ToString());
            list.Add(new Parameter(name));
         }

         var parameters = new Parameters(list);

         var lambda = new Lambda(new Region(), block, parameters, false);
         result.Value = lambda;
         Lambda = lambda;
         overridePosition = index;
         return new CreateLambda(parameters, block, false);
      }

      IMaybe<int> isStopping(int index)
      {
         if (freeParser.Scan(source, index, stop.Pattern))
         {
            if (stop.Consume)
            {
               freeParser.ColorAll(stop.Color);
               return freeParser.Position.Some();
            }

            return index.Some();
         }

         return none<int>();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, Structures);
         FillInVariableParser.Index = 0;
         var block = new Block();
         maxIndex = -1;

         IMaybe<int> newIndex;
         var index = NextPosition;

         if (index < source.Length)
         {
            newIndex = isStopping(index);
            if (newIndex.IsSome)
               return returnLambda(block, newIndex.Value);

            newIndex = getTerm(block, index);
            if (newIndex.IsNone)
               return null;

            index = newIndex.Value;
         }

         while (index < source.Length)
         {
            newIndex = isStopping(index);
            if (newIndex.IsSome)
               return returnLambda(block, newIndex.Value);

            if (operatorParser.Scan(source, index))
            {
               block.Add(operatorParser.Verb);
               index = operatorParser.Position;
            }
            else
               break;

            newIndex = getTerm(block, index);
            if (newIndex.IsNone)
               return null;

            index = newIndex.Value;
         }

         return returnLambda(block, index);
      }

      public override string VerboseName => "short lambda";

      public Lambda Lambda { get; set; }
   }
}
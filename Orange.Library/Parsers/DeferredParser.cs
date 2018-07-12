using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor;

namespace Orange.Library.Parsers
{
   public class DeferredParser : Parser
   {
      static Lambda createLambda(Verb verb)
      {
         var builder = new CodeBuilder();
         switch (verb.OperandCount)
         {
            case 2:
               builder.Parameter("$0");
               builder.Parameter("$1");
               builder.Variable("$0");
               builder.Verb(verb);
               builder.Variable("$1");
               return builder.Lambda();
            case 1:
               builder.Parameter("$0");
               builder.Variable("$0");
               builder.Verb(verb);
               return builder.Lambda();
            default:
               return null;
         }
      }

      Verb createLambda(ParserResult parserResult)
      {
         var verb = parserResult.Verb;
         if (verb == null || verb is NullOp)
            return null;
         var leftToRight = verb.LeftToRight;
         var lambda = createLambda(verb);
         if (lambda == null)
            return null;
         overridePosition = parserResult.Position;
         result.Value = lambda;
         return new Fold(lambda, leftToRight);
         //return new Push(lambda);
      }

      ThreeCharacterOperatorParser threeCharacterOperatorParser;
      TwoCharacterOperatorParser twoCharacterOperatorParser;
      OneCharacterOperatorParser oneCharacterOperatorParser;
      WordOperatorParser wordOperatorParser;
      VariableParser variableParser;

      public DeferredParser()
         : base("^ /(|sp| '`') -(> ' '+)")
      {
         threeCharacterOperatorParser = new ThreeCharacterOperatorParser();
         twoCharacterOperatorParser = new TwoCharacterOperatorParser();
         oneCharacterOperatorParser = new OneCharacterOperatorParser();
         wordOperatorParser = new WordOperatorParser();
         variableParser = new VariableParser();
      }

      public override Verb CreateVerb(string[] tokens)
      {
         Color(position, length, EntityType.Operators);
         var index = position + length;
         if (threeCharacterOperatorParser.Scan(source, index))
            return createLambda(threeCharacterOperatorParser.Result);
         if (twoCharacterOperatorParser.Scan(source, index))
            return createLambda(twoCharacterOperatorParser.Result);
         if (oneCharacterOperatorParser.Scan(source, index))
            return createLambda(oneCharacterOperatorParser.Result);
         if (wordOperatorParser.Scan(source, index))
            return createLambda(wordOperatorParser.Result);
         if (variableParser.Scan(source, index))
         {
            var parserResult = variableParser.Result;
            overridePosition = parserResult.Position;
            result.Value = parserResult.Value;
            return parserResult.Verb;
         }
         return null;
      }

      public override string VerboseName => "deferred";
   }
}
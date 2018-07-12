using Orange.Library.Values;
using Orange.Library.Verbs;
using Standard.Types.Strings;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class IntegerParser : Parser
   {
      public IntegerParser()
         : base("^ |sp| ['+-']? /([/d '_']+) /(['iLr'])?")
      {
      }

      public override Verb CreateVerb(string[] tokens)
      {
         if (tokens[1] == "_")
            return null;
         Color(position, length, Numbers);
         var numberString = tokens[0].Replace("_", "").Trim();
         switch (tokens[2])
         {
            case "L":
               result.Value = new Big(numberString.Replace("L", ""));
               return new Push(result.Value);
            case "r":
               result.Value = new Rational(numberString.Replace("r", "").ToInt(), 1);
               return new Push(result.Value);
            case "i":
               result.Value = new Complex(0, numberString.Replace("i", "").ToDouble());
               return new Push(result.Value);
         }
         var value = numberString.ToDouble();
         var number = new Double(value);
         result.Value = number;
         return new Push(number);
      }

      public override string VerboseName => "integer";
   }
}
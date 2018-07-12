using Orange.Library.Values;
using Orange.Library.Verbs;
using static Orange.Library.Parsers.IDEColor.EntityType;

namespace Orange.Library.Parsers
{
   public class AbstractParser : Parser
   {
      SignatureParser signatureParser;

      public AbstractParser()
         : base("^ /(|tabs|) /('abstract') /(/s+)") => signatureParser = new SignatureParser(false);

      public override Verb CreateVerb(string[] tokens)
      {
         if (!InClassDefinition)
            return null;

         Color(position, length, KeyWords);
         var index = position + length;
         if (signatureParser.Scan(source, index))
         {
            var signature = (Signature)signatureParser.Result.Value;
            var anAbstract = new Abstract(signature);
            result.Value = anAbstract;
            overridePosition = signatureParser.Result.Position;
            return new SpecialAssignment(signature.Name, anAbstract);
         }

         return null;
      }

      public override string VerboseName => "abstract";
   }
}
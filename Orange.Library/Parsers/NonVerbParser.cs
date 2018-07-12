using Orange.Library.Verbs;
using Standard.Types.Maybe;

namespace Orange.Library.Parsers
{
   public abstract class NonVerbParser<T> : Parser
   {
      public NonVerbParser()
         : base("")
      {
      }

      public override Verb CreateVerb(string[] tokens) => null;

      public override string VerboseName => "non verb";

      public abstract IMaybe<T> Parse();

      public override bool Scan(string source, int position)
      {
         this.source = source;
         this.position = position;
         var parsed = Parse();
         if (parsed.IsNone)
            return false;
         result.Verb = new NullOp();
         result.Position = overridePosition ?? position + length;
         Parsed = parsed.Value;
         return true;
      }

      public T Parsed
      {
         get;
         set;
      }
   }
}
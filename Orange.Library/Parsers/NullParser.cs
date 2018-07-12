using Orange.Library.Verbs;

namespace Orange.Library.Parsers
{
   public abstract class NullParser : Parser
   {
      protected NullParser()
         : base("")
      {
      }

      public override string VerboseName => "null";

      public abstract Verb Parse();

      public override bool Scan(string source, int position)
      {
         this.source = source;
         this.position = position;
         var verb = Parse();
         if (verb == null)
            return false;
         result.Verb = verb;
         result.Position = overridePosition ?? position + length;
         return true;
      }

      public override Verb CreateVerb(string[] tokens) => null;
   }
}
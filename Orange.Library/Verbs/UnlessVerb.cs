using Orange.Library.Values;

namespace Orange.Library.Verbs
{
   public class UnlessVerb : WhenVerb
   {
      public UnlessVerb(Verb verb, Block condition)
         : base(verb, condition)
      {
      }

      protected override bool isTrue() => !base.isTrue();

      public override string ToString() => $"{verb} unless {condition}";
   }
}
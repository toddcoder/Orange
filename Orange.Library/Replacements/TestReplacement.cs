using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Replacements
{
   public class TestReplacement : IReplacement
   {
      string character;
      long id;

      public TestReplacement(string character)
      {
         this.character = character;
         id = CompilerState.ObjectID();
      }

      public string Text => character.Repeat(State.WorkingInput.Length);

      public bool Immediate { get; set; }

      public long ID => id;

      public override string ToString() => $"?{character}";

      public void Evaluate() { }

      public IReplacement Clone() => new TestReplacement(character);

      public Arguments Arguments { get; set; }

      public IMaybe<long> FixedID { get; set; } = none<long>();
   }
}
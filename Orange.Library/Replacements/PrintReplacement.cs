using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Replacements
{
   public class PrintReplacement : IReplacement
   {
      long id;

      public PrintReplacement() => id = CompilerState.ObjectID();

      public override string ToString() => "!";

      public string Text
      {
         get
         {
            Evaluate();
            return null;
         }
      }

      public bool Immediate { get; set; }

      public long ID => id;

      public void Evaluate() => State.Print(State.WorkingInput);

      public IReplacement Clone() => new PrintReplacement
      {
         Immediate = Immediate,
         id = id
      };

      public Arguments Arguments { get; set; }

      public IMaybe<long> FixedID { get; set; } = none<long>();
   }
}
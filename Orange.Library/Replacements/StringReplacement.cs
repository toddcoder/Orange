using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Replacements
{
   public class StringReplacement : IReplacement
   {
      String text;
      long id;

      public StringReplacement(String text)
      {
         this.text = text;
         id = CompilerState.ObjectID();
      }

      public StringReplacement() => id = CompilerState.ObjectID();

      public string Text => text.Text;

      public bool Immediate { get; set; }

      public long ID => id;

      public void Evaluate() { }

      public IReplacement Clone() => new StringReplacement(text) { Immediate = Immediate };

      public Arguments Arguments { get; set; }

      public IMaybe<long> FixedID { get; set; } = none<long>();

      public override string ToString() => $"'{text}'";
   }
}
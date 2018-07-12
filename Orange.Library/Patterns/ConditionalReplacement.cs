using Orange.Library.Replacements;
using Standard.Types.Strings;

namespace Orange.Library.Patterns
{
   public class ConditionalReplacement
   {
      public int Index { get; set; }

      public int Length { get; set; }

      public IReplacement Replacement { get; set; }

      public long ID { get; set; }

      public override string ToString() => $"@({Index}, {Length})='{Runtime.State.WorkingInput.Sub(Index, Length)}'";
   }
}
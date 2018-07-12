using System.Linq;
using Orange.Library.Replacements;
using Orange.Library.Values;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Strings;
using Standard.Types.Numbers;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Patterns
{
   public class ConditionalReplacements
   {
      Hash<long, ConditionalReplacement> replacements;

      public ConditionalReplacements() => replacements = new Hash<long, ConditionalReplacement>();

      public void Add(int index, int length, IReplacement replacement)
      {
         var id = replacement.FixedID.FlatMap(i => i, () => CompilerState.ObjectID());
         var conditionalReplacement = new ConditionalReplacement
         {
            Index = index, Length = length, Replacement = replacement, ID = id
         };
         replacements[conditionalReplacement.ID] = conditionalReplacement;
      }

      public void Remove(IReplacement replacement) => replacements.Remove(replacement.ID);

      public void Replace()
      {
         var offset = 0;
         var slicer = new Slicer(State.Input);
         foreach (var item in replacements)
         {
            State.SaveWorkingInput();
            var replacement = item.Value;
            var workingInput = slicer[replacement.Index, replacement.Length];
            State.WorkingInput = workingInput;
            var inputValue = workingInput.IsNumeric() ? (Value)workingInput.ToDouble() : workingInput;
            var arguments = new Arguments();
            arguments.AddArgument(inputValue);
            arguments.AddArgument(replacement.Index);
            arguments.AddArgument(replacement.Length);
            replacement.Replacement.Arguments = arguments;

            var text = replacement.Replacement.Text;

            State.RestoreWorkingInput();

            if (text == null)
               continue;

            var oldLength = slicer.Length;
            slicer[replacement.Index, replacement.Length] = text;
            offset = slicer.Length - oldLength;
         }

         State.Input = slicer.ToString();
         replacements.Clear();
         State.Position += offset;
      }

      public override string ToString() => replacements.Select(i => i.Value.ToString()).Listify(" ");

      public void Clear() => replacements.Clear();
   }
}
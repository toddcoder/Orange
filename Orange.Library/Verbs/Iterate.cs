using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Iterate : Verb, IStatement
   {
      const string LOCATION = "Each";

      Parameters parameters;
      Block source;
      IMaybe<Block> first;
      Block middle;
      IMaybe<Block> last;
      Block block;
      string result;

      public Iterate(Parameters parameters, Block source, IMaybe<Block> first, Block middle, IMaybe<Block> last,
         Block block)
      {
         this.parameters = parameters;
         this.source = source;
         this.first = first;
         this.middle = middle;
         this.last = last;
         this.block = block;
         result = "";
      }

      public override Value Evaluate()
      {
         var generator = source.Evaluate().PossibleGenerator();
         Assert(generator.IsSome, LOCATION, "Generator required");
         var array = (Array)generator.Value.Array();
         if (array.Length == 0)
            return null;

         using (var popper = new RegionPopper(new Region(), "each"))
         {
            popper.Push();

            var length = array.Length;
            switch (length)
            {
               case 1:
                  evaluateFirst(array[0]);
                  result = "1 iteration";
                  return null;
               case 2:
                  evaluateFirst(array[0]);
                  evaluateLast(array[1], 1);
                  result = "2 iterations";
                  return null;
            }

            var lastIndex = length - 1;

            evaluateFirst(array[0]);

            for (var i = 1; i < lastIndex; i++)
               evaluateMiddle(array[i], i);

            evaluateLast(array[lastIndex], lastIndex);

            result = $"{array.Length} iterations";
            return null;
         }
      }

      void evaluateFirst(Value element)
      {
         parameters.SetValues(element, 0);
         block.Evaluate();
         if (first.If(out var b))
            b.Evaluate();
         else
            middle.Evaluate();
      }

      void evaluateMiddle(Value element, int index)
      {
         parameters.SetValues(element, index);
         block.Evaluate();
         middle.Evaluate();
      }

      void evaluateLast(Value element, int index)
      {
         parameters.SetValues(element, index);
         block.Evaluate();
         if (last.If(out var b))
            b.Evaluate();
         else
            middle.Evaluate();
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => "iterate";

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}
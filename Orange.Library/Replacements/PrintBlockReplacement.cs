using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Replacements
{
   public class PrintBlockReplacement : IReplacement
   {
      const string LOCATION = "Print block replacement";

      Lambda lambda;
      long id;

      public PrintBlockReplacement(Lambda lambda)
      {
         Assert(lambda.Parameters != null && lambda.Parameters.Length > 0, LOCATION, "No parameter provided");
         this.lambda = lambda;
         id = CompilerState.ObjectID();
      }

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

      public void Evaluate()
      {
         var text = lambda.Evaluate(Arguments).Text;
         State.Print(text);
      }

      public IReplacement Clone() => new PrintBlockReplacement((Lambda)lambda.Clone());

      public Arguments Arguments { get; set; }

      public IMaybe<long> FixedID { get; set; } = none<long>();

      public override string ToString() => $"{{{lambda}}}!";
   }
}
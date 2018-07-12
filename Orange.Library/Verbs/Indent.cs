using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class Indent : Verb, IStatement
   {
      Block expression;
      bool relative;
      int factor;
      string result;

      public Indent(Block expression, bool relative, int factor)
      {
         this.expression = expression;
         this.relative = relative;
         this.factor = factor;
         result = "";
      }

      public override Value Evaluate()
      {
         var i = expression.Evaluate().Int;
         if (relative)
            State.IndentBy(factor * i);
         else
            State.Indent(i);
         result = $"indentation is {State.Indentation().Replace("\t", "`t")}";
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"indent {(relative && factor > 0 ? "+" : "")}{expression}";

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}
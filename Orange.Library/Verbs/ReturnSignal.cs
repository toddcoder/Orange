using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Verbs
{
   public class ReturnSignal : Verb, ITailCallVerb, IStatement
   {
      IMaybe<Block> expression;
      string typeName;

      public ReturnSignal()
      {
         expression = none<Block>();
         typeName = "";
      }

      public ReturnSignal(Block expression)
      {
         this.expression = expression.Some();
         typeName = "";
      }

      public ReturnSignal(Generator generator)
      {
         expression = generator.Pushed.Some();
         typeName = "";
      }

      public override Value Evaluate()
      {
         var value = expression.FlatMap(e => e.Evaluate().AssignmentValue(), () => new Nil());
         typeName = value.Type.ToString();
         State.ReturnValue = value;
         State.ReturnSignal = true;
         return value;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => "return";

      public TailCallSearchType TailCallSearchType => TailCallSearchType.TailCallVerb;

      public string NameProperty => null;

      public Block NestedBlock => null;

      public string Result => expression.FlatMap(e => e.ToString(), () => "");

      public string TypeName => typeName;

      public int Index { get; set; }
   }
}
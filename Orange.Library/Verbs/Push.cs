using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Verbs
{
   public class Push : Verb, ITailCallVerb
   {
      protected Value value;

      public Push(Value value) => this.value = value;

      public Push()
         : this("") { }

      public override Value Evaluate() => value;

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => value.ToString();

      public Value Value => value;

      public TailCallSearchType TailCallSearchType => Value is Block ? TailCallSearchType.NestedBlock :
         TailCallSearchType.Cancel;

      public string NameProperty => null;

      public Block NestedBlock => TailCallSearchType == TailCallSearchType.NestedBlock ? (Block)value : null;

      public IMaybe<string> Variable() => value is Variable v ? v.Name.Some() : none<string>();

      public override IMaybe<Value.ValueType> PushType => value.Type.Some();

      public override AffinityType Affinity => AffinityType.Value;
   }
}
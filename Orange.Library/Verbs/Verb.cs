using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.ExpressionManager;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Verbs
{
   public abstract class Verb
   {
      public enum AffinityType
      {
         Prefix,
         Postfix,
         Infix,
         Value
      }

      public abstract Value Evaluate();

      public abstract VerbPresidenceType Presidence { get; }

      public virtual bool LeftToRight => true;

      public string VerbName => $"verb-{CompilerState.ObjectID()}";

      public bool IsOperator { get; set; }

      public virtual int OperandCount => 2;

      public virtual AffinityType Affinity => AffinityType.Infix;

      public virtual IMaybe<Value.ValueType> PushType => none<Value.ValueType>();

      public int LineNumber { get; set; }

      public int LinePosition { get; set; }

      public virtual IMaybe<INSGenerator> PossibleGenerator()
      {
         if (Yielding && this is INSGeneratorSource gs)
            return gs.GetGenerator().Some();

         return none<INSGenerator>();
      }

      public virtual bool Yielding => false;
   }
}
using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Values.Object;
using static Orange.Library.Values.Object.VisibilityType;

namespace Orange.Library.Verbs
{
   public class ParameterModifiers : Verb
   {
      public override Value Evaluate() => null;

      public override VerbPresidenceType Presidence => VerbPresidenceType.PreIncrement;

      public VisibilityType VisibilityType
      {
         get;
         set;
      } = Public;

      public bool ReadOnly
      {
         get;
         set;
      }
   }
}
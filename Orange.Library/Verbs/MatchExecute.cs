using Orange.Library.Values;
using Standard.Types.Strings;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class MatchExecute : Verb, IStatement
   {
      Block target;
      Block actions;
      string fieldName;
      string result;
      string typeName;

      public MatchExecute(Block target, Block actions, string fieldName)
      {
         this.target = target;
         this.actions = actions;
         this.fieldName = fieldName;
         result = "";
         typeName = "";
      }

      public override Value Evaluate()
      {
         var value = target.Evaluate();
         var current = Regions.Current;
         actions.AutoRegister = false;
         State.RegisterBlock(actions);
         State.Stack.Push(value);
         var evaluated = actions.Evaluate();
         if (fieldName.IsNotEmpty())
            if (Regions.FieldExists(fieldName))
               Regions[fieldName] = evaluated.AssignmentValue();
            else
               current.CreateAndSet(fieldName, evaluated.AssignmentValue());
         result = value.ToString();
         typeName = value.Type.ToString();
         State.UnregisterBlock();
         return evaluated;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public string Result => result;

      public string TypeName => typeName;

      public int Index { get; set; }

      public override string ToString() => $"match {target} {actions}";
   }
}
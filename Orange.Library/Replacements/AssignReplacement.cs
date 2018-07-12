using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Replacements
{
   public class AssignReplacement : IReplacement
   {
      string variableName;
      long id;
      bool local;
      bool bind;

      public AssignReplacement(string variableName, bool local, bool bind)
      {
         this.variableName = variableName;
         this.local = local;
         this.bind = bind;
         id = CompilerState.ObjectID();
      }

      public AssignReplacement() => id = CompilerState.ObjectID();

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
         var input = State.WorkingInput;
         if (Immediate)
            Regions.Current.SetParameter(variableName, input);
         else if (bind)
            State.BindToPattern(variableName, input);
         else
         {
            var current = Regions?.GrandParent() ?? Regions.Current;
            if (IsSpecialVariable(variableName))
               Regions[variableName] = input;
            else
            {
               current.CreateVariableIfNonexistant(variableName);
               current[variableName] = input;
            }
         }
      }

      public IReplacement Clone() => new AssignReplacement(variableName, local, bind)
      {
         Immediate = Immediate
      };

      public Arguments Arguments { get; set; }

      public IMaybe<long> FixedID { get; set; } = none<long>();

      public override string ToString() => variableName;
   }
}
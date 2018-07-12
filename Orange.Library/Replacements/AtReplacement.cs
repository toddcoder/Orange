using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Compiler;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Replacements
{
   public class AtReplacement : IReplacement
   {
      string variableName;
      long id;
      bool push;

      public AtReplacement(string variableName, bool push)
      {
         this.variableName = variableName;
         this.push = push;
         id = CompilerState.ObjectID();
      }

      public AtReplacement() => id = CompilerState.ObjectID();

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
         var at = Arguments[1].Number;
         if (push)
         {
            var variable = new Variable(variableName);
            SendMessage(variable, "push", at);
         }
         else
            Regions[variableName] = at;
      }

      public override string ToString() => $"={variableName}";

      public IReplacement Clone() => new AtReplacement(variableName, push);

      public Arguments Arguments { get; set; }

      public IMaybe<long> FixedID { get; set; } = none<long>();
   }
}
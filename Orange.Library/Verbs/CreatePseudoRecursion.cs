using Orange.Library.Values;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Managers.RegionManager;

namespace Orange.Library.Verbs
{
   public class CreatePseudoRecursion : Verb, IStatement
   {
      string name;
      Parameters parameters;
      Block block;
      string result;

      public CreatePseudoRecursion(string name, Parameters parameters, Block block)
      {
         this.name = name;
         this.parameters = parameters;
         this.block = block;
         result = "";
      }

      public override Value Evaluate()
      {
         PseudoRecursion pseudoRecursion;
         if (Regions.VariableExists(name))
         {
            var value = Regions[name];
            if (!(value is PseudoRecursion))
            {
               pseudoRecursion = new PseudoRecursion(name);
               Regions[name] = pseudoRecursion;
            }
            else
               pseudoRecursion = (PseudoRecursion)value;
         }
         else
         {
            pseudoRecursion = new PseudoRecursion(name);
            Regions.CreateVariable(name);
            Regions[name] = pseudoRecursion;
         }
         if (parameters.Length == 0)
            pseudoRecursion.Initialization = block;
         else
         {
            var parameterBlock = new ParameterBlock(parameters, block, parameters.Splatting);
            if (pseudoRecursion.TerminalExpression == null)
               pseudoRecursion.TerminalExpression = parameterBlock;
            else
            {
               pseudoRecursion.Main = parameterBlock;
               pseudoRecursion.Initialize();
            }
         }
         result = ToString();
         return null;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Statement;

      public override string ToString() => $"rec {name}({parameters}){{{block}}}";

      public string Result => result;

      public string TypeName => "";

      public int Index { get; set; }
   }
}
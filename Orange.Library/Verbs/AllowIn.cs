using System.Collections.Generic;
using Orange.Library.Parsers.Special;
using Orange.Library.Values;
using Standard.Types.Maybe;
using static Orange.Library.Managers.ExpressionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Verbs
{
   public class AllowIn : Verb
   {
      Block definition;
      Block evaluation;

      public AllowIn(Block definition, Block evaluation)
      {
         this.definition = definition;
         this.definition.AutoRegister = false;
         this.evaluation = evaluation;
         this.evaluation.AutoRegister = false;
      }

      public override Value Evaluate()
      {
         var region = new ObjectRegion(null);
         State.RegisterBlock(definition, region);
         var parser = new ParameterListParser();
         var parsed = parser.Parse(definition);
         List<Parameter> parameterList;
         Assert(parsed.Assign(out parameterList), "Allow In", "Invalid parameter list");
         foreach (var parameter in parameterList)
         {
            var value = parameter.DefaultValue.Evaluate();
            region.SetReadOnly(parameter.Name, value);
         }
         State.UnregisterBlock();
         var lockedDownRegion = new LockedDownRegion(region, "");
         State.RegisterBlock(evaluation, lockedDownRegion);
         var result = evaluation.Evaluate();
         State.UnregisterBlock();
         return result;
      }

      public override VerbPresidenceType Presidence => VerbPresidenceType.Push;

      public override string ToString() => $"allow ({definition}) in ({evaluation})";
   }
}
namespace Orange.Library.Values
{
   public class CurryingLambda : Lambda
   {
      public CurryingLambda(Region region, Block block, Parameters parameters, bool enclosing)
         : base(region, block, parameters, enclosing) { }

      public override Value Evaluate(Arguments arguments, Value instance = null, bool register = true, bool setArguments = true)
      {
         if (parameters.Length <= 1)
            return base.Evaluate(arguments, instance, register, setArguments);

         var allParameters = parameters.GetParameters();
         var firstParameter = allParameters[0];
         var blocks = arguments.Blocks;
         var passedValue = blocks.Length > 0 ? blocks[0] : firstParameter.DefaultValue;
         passedValue.Expression = true;
         var builder = new CodeBuilder();
         builder.AssignToNewField(true, firstParameter.Name, passedValue);
         builder.End();
         for (var i = 1; i < allParameters.Length; i++)
         {
            var parameter = allParameters[i];
            builder.Parameter(parameter);
         }

         builder.Inline(block);
         var curryingLambda = builder.CurryingLambda();
         if (blocks.Length > 1)
         {
            var newArguments = new Arguments();
            for (var i = 1; i < blocks.Length; i++)
            {
               var value = blocks[i].Evaluate();
               if (value is IInvokeable invokeable)
                  value = invokeable.Invoke(new Arguments());
               newArguments.AddArgument(value);
            }

            return curryingLambda.Evaluate(newArguments, instance, register, setArguments);
         }

         return curryingLambda;
      }
   }
}
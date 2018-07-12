using Orange.Library.Values;
using System.Linq;

namespace Orange.Library
{
   public static class ArgumentHelper
   {
      public static Array ToDefinedArguments(this Block argumentsAsBlock)
      {
         argumentsAsBlock.ResolveVariables = false;
         var arguments = argumentsAsBlock.Evaluate();
         if (arguments == null)
            return new Array();
         if (arguments.Type == Value.ValueType.Array)
            return getVariables((Array)arguments.Resolve());
         return new Array
          {
             getVariable(arguments)
          };
      }

      static Variable getVariable(Value value)
      {
         if (value.IsVariable)
            return (Variable)value;
         return new Variable(value.Text);
      }

      static Array getVariables(Array array) => new Array(array.Values.Select(getVariable));

      public static Array ToActualArguments(this Block argumentsAsBlock)
      {
         argumentsAsBlock.ResolveVariables = false;
         var arguments = argumentsAsBlock.Evaluate();
         if (arguments == null)
            return new Array();
         arguments = arguments.Resolve();
         if (arguments.IsArray)
            arguments = arguments.SourceArray;
         else
            arguments = new Array
            {
               arguments
            };
         return (Array)arguments;
      }

      public static Array ToActualSliceArguments(this Block argumentsAsBlock)
      {
         argumentsAsBlock.ResolveVariables = false;
         var arguments = argumentsAsBlock.Evaluate();
         if (arguments == null)
            return new Array();
         arguments = arguments.Resolve();
         if (arguments.IsArray)
            arguments = arguments.SliceArray;
         else
            arguments = new Array
            {
               arguments
            };
         return (Array)arguments;
      }
   }
}
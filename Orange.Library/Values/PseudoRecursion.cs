using System.Collections.Generic;
using Orange.Library.Managers;
using Orange.Library.Verbs;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class PseudoRecursion : Value
   {
      const string LOCATION = "Pseudo recursion";

      string name;
      Block block;
      Block initialization;
      Block checkExpression;
      Block invariant;
      Block increment;
      string mainParameterName;
      Parameters parameters;
      Block arrayComparisand;
      Block terminalExpression;
      bool useTerminalExpression;

      public PseudoRecursion(string name)
      {
         this.name = name;
         block = new Block();
         useTerminalExpression = false;
      }

      public PseudoRecursion(string name, Block block, bool useTerminalExpression)
      {
         this.name = name;
         this.block = block;
         this.useTerminalExpression = useTerminalExpression;
      }

      public Block Initialization { get; set; }

      public ParameterBlock TerminalExpression { get; set; }

      public ParameterBlock Main { get; set; }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get => "";
         set { }
      }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.PseudoRecursion;

      public override bool IsTrue => false;

      public override Value Clone() => new PseudoRecursion(name, (Block)block.Clone(), useTerminalExpression);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "invoke", v => ((PseudoRecursion)v).Invoke());
      }

      public void Initialize()
      {
         terminalExpression = TerminalExpression.Block;
         var parameter = TerminalExpression.Parameters[0];
         mainParameterName = parameter.Name;

         block = Main.Block;
         var builder = new CodeBuilder();
         builder.Define(VAR_ACCUM);
         builder.Assign();
         if (Initialization != null)
            builder.Parenthesize(Initialization);
         else
         {
            var defaultValue = parameter.DefaultValue;
            if (defaultValue != null && defaultValue.Count > 0)
               builder.Parenthesize(defaultValue);
            else
               builder.Variable(parameter.Name);
         }
         initialization = builder.Block;
         initialization.AutoRegister = false;

         var comparisand = TerminalExpression.Parameters[0].Comparisand;
         RejectNull(comparisand, LOCATION, "No check expression provided");
         checkExpression = comparisand;
         checkExpression.AutoRegister = false;

         var found = false;
         Arguments arguments = null;
         builder = new CodeBuilder();
         builder.Variable(VAR_ACCUM);
         builder.Assign();
         for (var i = 0; i < block.Count; i++)
            if (CodeBuilder.FunctionInvoke(block, ref i, out var functionName, out var foundArguments) && functionName == name)
            {
               Reject(found, LOCATION, $"{name} already invoked");
               found = true;
               arguments = foundArguments;
               builder.Variable(VAR_ACCUM);
            }
            else
               builder.Verb(block[i]);

         invariant = builder.Block;
         invariant.AutoRegister = false;
         Assert(found, LOCATION, $"Didn't find any function invocations for {name}");
         increment = arguments.ArgumentsBlock;
         increment.AutoRegister = false;

         builder = new CodeBuilder();
         var setup = true;
         var parameterIndex = 0;
         parameters = Main.Parameters;
         arrayComparisand = parameters[0].Comparisand;
         foreach (var verb in increment)
         {
            if (setup)
            {
               var length = parameters.Length;
               Assert(parameterIndex < length, LOCATION, "No more parameters");
               builder.Define(VAR_MANGLE + parameters[parameterIndex++].Name);
               builder.Assign();
               setup = false;
            }
            if (verb is AppendToArray)
            {
               builder.End();
               setup = true;
            }
            else
               builder.Verb(verb);
         }

         builder.End();
         foreach (var variableName in parameters.VariableNames)
         {
            builder.Variable(variableName);
            builder.Assign();
            builder.Variable(VAR_MANGLE + variableName);
            builder.End();
         }

         increment = builder.Block;
         increment.AutoRegister = false;

         builder = new CodeBuilder();
         builder.Variable(VAR_ACCUM);
         builder.Assign();
         builder.Push();
         foreach (var verb in terminalExpression)
         {
            string variableName;
            if (CodeBuilder.PushVariable(verb, out variableName))
            {
               useTerminalExpression = true;
               builder.Variable(variableName == name ? VAR_ACCUM : variableName);
            }
            else
               builder.Verb(verb);
         }

         terminalExpression = builder.Pop(true);
         builder.Parenthesize(terminalExpression);
         terminalExpression = builder.Block;
         terminalExpression.AutoRegister = false;
      }

      public Value Invoke()
      {
         var region = new Region();
         using (var popper = new RegionPopper(region, "pseudo-recursion"))
         {
            List<ParameterValue> values = null;
            if (parameters != null)
               values = parameters.GetArguments(Arguments);
            popper.Push();
            if (values != null)
               Parameters.SetArguments(values);
            initialization.Evaluate(region);
            var failedArrayComparison = false;
            for (var i = 0; i < MAX_TAILCALL; i++)
            {
               var value = region[mainParameterName];

               if (arrayComparisand != null)
               {
                  var right = arrayComparisand.Evaluate(region);
                  if (!Case.Match(value, right, region, false, false, parameters.Condition))
                  {
                     if (right.IsArray)
                     {
                        var array = (Array)right.SourceArray;
                        foreach (var item in array)
                           if (item.Value is Placeholder placeholder)
                           {
                              var variableName = placeholder.Text;
                              region.CreateVariableIfNonexistant(variableName);
                              region[variableName] = new Array();
                           }
                     }

                     failedArrayComparison = true;
                  }
               }

               var comparisand = checkExpression.Evaluate(region);
               if (failedArrayComparison || comparisand.Type == ValueType.Boolean && comparisand.IsTrue ||
                  Case.Match(value, comparisand, region, false, false, parameters.Condition))
                  return useTerminalExpression ? terminalExpression.Evaluate(region) : region[VAR_ACCUM];

               increment.Evaluate(region);
               invariant.Evaluate(region);
            }

            Throw(LOCATION, "Exceed recursion limit");
            return null;
         }
      }

      public override string ToString() => $"init({initialization}) inc({increment}) inv({invariant}) " +
         $"trm({terminalExpression}) chk({checkExpression})";
   }
}
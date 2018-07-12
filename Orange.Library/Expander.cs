using System.Linq;
using Orange.Library.Values;
using Standard.Types.Collections;
using static Orange.Library.CodeBuilder;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;
using Value = Orange.Library.Values.Value;

namespace Orange.Library
{
   public class Expander
   {
      class ParameterData
      {
         string name;
         Value value;
         Block comparisand;

         public ParameterData(string name, Value value, Block comparisand)
         {
            this.name = name;
            this.value = value;
            this.comparisand = comparisand;
            Incrementer = null;
         }

         public string Name => name;

         public string MangledName => MangledName(name);

         public Value Value => value;

         public Block Comparisand => comparisand;

         public Block Incrementer { get; set; }
      }

      string functionName;
      Block block;
      Block checkExpression;
      string checkExpressionVariable;
      Block terminalExpression;
      Region region;
      Hash<string, ParameterData> allParameterData;
      CodeBuilder builder;
      Block result;
      Block sourceBlock;

      public Expander(string functionName, Block block, Block checkExpression, string checkExpressionVariable,
         Block terminalExpression, Region region)
      {
         this.functionName = functionName;
         this.block = block;
         this.checkExpression = checkExpression;
         this.checkExpressionVariable = checkExpressionVariable;
         this.terminalExpression = terminalExpression;
         this.region = region;
         allParameterData = new Hash<string, ParameterData>();
      }

      public void AddParameter(Parameter parameter)
      {
         var value = region[parameter.Name];
         allParameterData[parameter.Name] = new ParameterData(parameter.Name, value, parameter.Comparisand);
      }

      public Block Expand()
      {
         using (var popper = new RegionPopper(new Region(), "expand"))
         {
            popper.Push();

            foreach (var data in allParameterData.Select(item => item.Value))
            {
               Regions.SetParameter(data.Name, data.Value);
               if (data.Comparisand == null)
                  continue;

               var right = data.Comparisand.Evaluate(region);
               if (Case.Match(data.Value, right, false, null))
                  continue;

               return new Block();
            }

            builder = new CodeBuilder();
            createSourceBlock();

            builder = new CodeBuilder();
            for (var i = 0; i < MAX_RECURSION; i++)
            {
               expand(sourceBlock);
               variableExpand();
               increment();

               if (terminated(out var finalBlock))
                  return finalBlock;

               result = builder.Block;
               builder = new CodeBuilder();
            }

            return null;
         }
      }

      void increment()
      {
         foreach (var item in allParameterData)
         {
            var data = item.Value;
            if (data.Incrementer == null)
               continue;

            var value = data.Incrementer.Evaluate(region);
            region.SetParameter(data.MangledName, value);
         }

         foreach (var data in allParameterData.Select(item => item.Value).Where(data => data.Incrementer != null))
            region[data.Name] = region[data.MangledName];

         foreach (var variableName in allParameterData
            .Select(item => new
            {
               item,
               data = item.Value
            })
            .Where(t => t.data.Comparisand != null)
            .Select(t => new
            {
               t,
               value = region[t.item.Key]
            })
            .Select(t => new
            {
               t,
               right = t.t.data.Comparisand.Evaluate(region)
            })
            .Where(t => !Case.Match(t.t.value, t.right, region, false, false, null))
            .Where(t => t.right.IsArray)
            .Select(t => (Array)t.right.SourceArray)
            .SelectMany(a => a
               .Select(i => i.Value)
               .Where(v => v.Type == Value.ValueType.Placeholder)
               .Select(v => v.Text)))
            Regions.SetParameter(variableName, new Array());
      }

      void createSourceBlock()
      {
         var asAdded = block.AsAdded;
         var incrementerToBeCreated = true;
         for (var i = 0; i < block.Count; i++)
         {
            var verb = asAdded[i];
            if (FunctionInvoke(block, ref i, out var variableName, out var arguments))
               if (variableName == functionName)
                  if (incrementerToBeCreated)
                  {
                     var blocks = arguments.Blocks;
                     var length = blocks.Length;
                     var index = 0;
                     foreach (var item in allParameterData.TakeWhile(item => index < length))
                     {
                        item.Value.Incrementer = blocks[index++];
                        item.Value.Incrementer.AutoRegister = false;
                     }

                     builder.Variable(functionName);
                     incrementerToBeCreated = false;
                  }
                  else
                     builder.Variable(variableName);
               else
                  builder.Verb(verb);
            else
               builder.Verb(verb);
         }

         sourceBlock = builder.Block;
         result = (Block)sourceBlock.Clone();
      }

      void expand(Block expression)
      {
         foreach (var verb in result.AsAdded)
            if (PushVariable(verb, out var variableName))
               if (variableName == functionName)
                  builder.Inline(expression);
               else
               {
                  var value = Regions[variableName];
                  builder.Value(value);
               }
            else
               builder.Verb(verb);
      }

      bool terminated(out Block finalResult)
      {
         var right = checkExpression.Evaluate(region);
         if (right == null)
         {
            finalResult = new Block();
            return false;
         }

         switch (right.Type)
         {
            case Value.ValueType.Boolean:
            case Value.ValueType.Ternary:
               if (right.IsTrue)
               {
                  evaluateTerminalExpression(out finalResult);
                  return true;
               }

               break;
            default:
               var left = region[checkExpressionVariable];
               if (Case.Match(left, right, region, false, false, null))
               {
                  evaluateTerminalExpression(out finalResult);
                  return true;
               }

               break;
         }

         finalResult = null;
         return false;
      }

      void evaluateTerminalExpression(out Block finalResult)
      {
         variableExpand();
         builder = new CodeBuilder();
         builder.Push();
         var expression = terminalExpression.Evaluate();
         builder.Value(expression);
         var terminalBlock = builder.Pop(false);
         finalExpand(terminalBlock);
         finalResult = builder.Block;
      }

      void variableExpand()
      {
         builder.Push();
         foreach (var verb in result.AsAdded)
            if (PushVariable(verb, out var variableName) && variableName != functionName)
            {
               var value = Regions[variableName];
               builder.Value(value);
            }
            else
               builder.Verb(verb);

         result = builder.Pop(true);
      }

      void finalExpand(Block terminalBlock)
      {
         foreach (var verb in result.AsAdded)
            if (PushVariable(verb, out var variableName) && variableName == functionName)
               builder.Inline(terminalBlock);
            else
               builder.Verb(verb);
      }
   }
}
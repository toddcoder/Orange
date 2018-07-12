using Orange.Library.Managers;
using Standard.Types.Strings;
using System.Linq;
using Standard.Types.Enumerables;
using static System.Math;

namespace Orange.Library.Values
{
   public class BlockFormat : Value, IStringify
   {
      Parameters parameters;
      Block block;

      public BlockFormat(Parameters parameters, Block block)
      {
         this.parameters = parameters;
         this.block = block;
         this.block.AutoRegister = false;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get
         {
            return ToString();
         }
         set
         {
         }
      }

      public override double Number
      {
         get
         {
            return Text.ToDouble();
         }
         set
         {
         }
      }

      public override ValueType Type => ValueType.Format;

      public override bool IsTrue => false;

      public override Value Clone() => new BlockFormat((Parameters)parameters.Clone(), (Block)block.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "apply", v => ((BlockFormat)v).Apply());
         manager.RegisterMessage(this, "invoke", v => ((BlockFormat)v).Invoke());
      }

      Value text(string[] newParameters, string[] arguments)
      {
         Runtime.State.RegisterBlock(block);
         var minLength = Min(newParameters.Length, arguments.Length);
         for (var i = 0; i < minLength; i++)
            RegionManager.Regions.SetLocal(newParameters[i], arguments[i]);
         var result = block.Evaluate();
         if (result.IsArray)
            result = ((Array)result.SourceArray).Values.Select(v => v.Text).Listify("");
         Runtime.State.UnregisterBlock();

         return result;
      }

      public Value Apply()
      {
         var argument = Arguments.ApplyValue;
         var array = argument.IsArray ? (Array)argument.SourceArray : new Array
         {
            argument
         };
         var variableNames = parameters.VariableNames;
         return text(variableNames, array.Values.Select(v => v.Text).ToArray());
      }

      public Value Invoke() => text(parameters.VariableNames, Arguments.Values.Select(v => v.Text).ToArray());

      public override string ToString() => $"{parameters} ? {{{block}}}";

      public Parameters Parameters => parameters;

      public Block Block => block;

      public Value String => Text;
   }
}
using Orange.Library.Managers;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Match : Value
   {
      const string LOCATION = "Match";

      Value value;
      bool found;
      Value result;

      public Match(Value value)
      {
         this.value = value;
         found = false;
         result = "";
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return result.Text; }
         set { }
      }

      public override double Number
      {
         get { return result.Number; }
         set { }
      }

      public override ValueType Type => ValueType.Match;

      public override bool IsTrue => found;

      public override Value Clone() => new Match(value.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "when", v => ((Match)v).When());
         manager.RegisterMessage(this, "result", v => ((Match)v).Result());
      }

      Value evaluate(Value block)
      {
         found = true;
         return block;
      }

      public Value Evaluate(Block block)
      {
         block.AutoRegister = false;
         State.RegisterBlock(block);
         Regions.SetLocal(VAL_MATCH_VALUE, this);
         result = block.Evaluate();
         State.UnregisterBlock();
         var array = result as Array;
         if (array == null)
            return "";

         foreach (var item in array)
            switch (item.Value.Type)
            {
               case ValueType.Block:
                  block = (Block)item.Value;
                  return block.Evaluate();
               case ValueType.Nil:
                  continue;
               default:
                  return item.Value;
            }

         return "";
      }

      public Value When()
      {
         if (found)
            return new Nil();

         var matched = Arguments[0];
         var block = Arguments[1];
         if (matched.IsArray)
            matched = matched.SourceArray;
         switch (matched.Type)
         {
            case ValueType.Array:
               var array = (Array)matched;
               if (array.ContainsValue(value))
                  return evaluate(block);

               break;
            case ValueType.Pattern:
               var pattern = (Pattern)matched;
               if (pattern.IsMatch(value.Text))
                  return evaluate(block);

               break;
            case ValueType.Lambda:
               var closure = (Lambda)matched;
               if (closure.Evaluate(new Arguments(value)).IsTrue)
                  return evaluate(block);

               break;
            case ValueType.Block:
               var matchBlock = (Block)matched;
               if (matchBlock.Evaluate().IsTrue)
                  return evaluate(block);

               break;
            case ValueType.Boolean:
               if (matched.IsTrue)
                  return evaluate(block);

               break;
            default:
               if (matched.Compare(value) == 0)
                  return evaluate(block);

               break;
         }

         return new Nil();
      }

      public Value Result() => result;
   }
}
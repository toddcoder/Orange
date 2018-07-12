using Orange.Library.Managers;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Array;

namespace Orange.Library.Values
{
   public class CFor : Value, ISequenceSource
   {
      Value seed;
      ParameterBlock increment;
      ParameterBlock whileBlock;
      Value current;

      public CFor(Value seed, ParameterBlock whileBlock, ParameterBlock increment)
      {
         this.seed = seed;
         this.increment = increment;
         this.whileBlock = whileBlock;
         current = null;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; } = "";

      public override double Number { get; set; } = 0;

      public override ValueType Type => ValueType.CFor;

      public override bool IsTrue => false;

      public override Value Clone() => new CFor(seed.Clone(), whileBlock.Clone(), increment.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((CFor)v).Map());
         manager.RegisterMessage(this, "if", v => ((CFor)v).If());
         manager.RegisterMessage(this, "unless", v => ((CFor)v).Unless());
         manager.RegisterMessage(this, "take", v => ((CFor)v).Take());
         manager.RegisterMessage(this, "next", v => ((CFor)v).Next());
         manager.RegisterMessage(this, "reset", v => ((CFor)v).Reset());
         manager.RegisterMessage(this, "invoke", v => ((CFor)v).Invoke());
         manager.RegisterMessage(this, "for", v => ((CFor)v).For());
      }

      public Value Map() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.Map();

      public Value If() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.If();

      public Value Unless() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.Unless();

      public Value Take() => new Sequence(this)
      {
         Arguments = Arguments.Clone()
      }.Take();

      public Value Next()
      {
         if (current == null)
         {
            current = seed;
            return current;
         }

         using (var whileAssistant = new ParameterAssistant(whileBlock, true))
         using (var incrementAssistant = new ParameterAssistant(increment, true))
         {
            var block = incrementAssistant.Block();
            if (block == null)
               return new Nil();

            incrementAssistant.IteratorParameter();
            incrementAssistant.SetIteratorParameter(current);
            current = block.Evaluate();
            whileAssistant.IteratorParameter();
            whileAssistant.SetIteratorParameter(current);
            block = whileAssistant.Block();
            if (block == null)
               return new Nil();
            if (block.Evaluate().IsTrue)
               return current;
         }

         return new Nil();
      }

      public ISequenceSource Copy() => (ISequenceSource)Clone();

      public Value Reset()
      {
         current = null;
         return this;
      }

      public int Limit => MAX_ARRAY;

      public Array Array => ArrayFromSequence(this);

      public override string ToString() => $"{seed}; {whileBlock}; {increment}";

      public Value Invoke() => Array;

      public override Value AlternateValue(string message) => Invoke();

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            assistant.IteratorParameter();
            Reset();
            var value = Next();
            var index = 0;
            while (!value.IsNil && index++ <= MAX_LOOP)
            {
               assistant.SetIteratorParameter(value);
               block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case Continuing:
                     value = Next();
                     continue;
                  case ReturningNull:
                     return null;
               }

               value = Next();
            }
         }

         return this;
      }
   }
}
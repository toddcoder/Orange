using Orange.Library.Managers;

namespace Orange.Library.Values
{
   public class SequenceComprehension : Value, ISequenceSource
   {
      Block block;
      SequenceComprehension innerComprehension;
      Parameters parameters;
      Block arrayBlock;
      Block ifBlock;
      Region region;
      int index;
      Value arrayValue;
      ISequenceSource sequence;

      public SequenceComprehension(Block block, Parameters parameters, Region region = null)
      {
         this.block = block;
         this.parameters = parameters;
         if (region == null)
         {
            this.region = new Region();
            RegionManager.Regions.Current.CopyAllVariablesTo(this.region);
         }
         else
            this.region = region;
         index = -1;
         arrayValue = null;
         sequence = null;
      }

      public SequenceComprehension(SequenceComprehension innerComprehension, Parameters parameters)
      {
         this.innerComprehension = innerComprehension;
         this.parameters = parameters;
         index = -1;
         arrayValue = null;
         sequence = null;
      }

      public SequenceComprehension(Comprehension comprehension)
      {
         block = comprehension.Block;
         arrayBlock = comprehension.ArrayBlock;
         parameters = comprehension.Parameters;
         region = comprehension.Region;
         if (comprehension.InnerComprehension != null)
            innerComprehension = new SequenceComprehension(comprehension.InnerComprehension);
         index = -1;
         arrayValue = null;
         sequence = null;
      }

      public Block ArrayBlock
      {
         get => arrayBlock;
         set => arrayBlock = value;
      }

      public Block IfBlock
      {
         get => ifBlock;
         set => ifBlock = value;
      }

      public override int Compare(Value value) => 0;

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Comprehension;

      public override bool IsTrue => false;

      public override Value Clone()
      {
         var clonedParameter = (Parameters)parameters.Clone();
         if (block != null)
         {
            var clonedBlock = (Block)block.Clone();
            return new SequenceComprehension(clonedBlock, clonedParameter, region.Clone())
            {
               ArrayBlock = (Block)arrayBlock.Clone(),
               IfBlock = (Block)ifBlock.Clone()
            };
         }

         return new SequenceComprehension((SequenceComprehension)innerComprehension.Clone(), clonedParameter)
         {
            ArrayBlock = (Block)arrayBlock.Clone(),
            IfBlock = (Block)ifBlock.Clone()
         };
      }

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "if", v => ((SequenceComprehension)v).If());
         manager.RegisterMessage(this, "map", v => ((SequenceComprehension)v).Map());
         manager.RegisterMessage(this, "next", v => ((SequenceComprehension)v).Next());
         manager.RegisterMessage(this, "reset", v => ((SequenceComprehension)v).Reset());
      }

      Value getNext()
      {
         if (arrayBlock == null)
            return new Nil();

         if (arrayValue == null)
         {
            arrayValue = arrayBlock.Evaluate();
            if (arrayValue == null || arrayValue.IsNil)
               return new Nil();

            sequence = arrayValue as ISequenceSource;
            if (sequence == null)
               if (arrayValue.IsArray)
                  arrayValue = arrayValue.SourceArray;
               else
                  return new Nil();
         }

         if (sequence != null)
         {
            index++;
            return sequence.Next();
         }

         var array = (Array)arrayValue;
         return ++index < array.Length ? array[index] : new Nil();
      }

      public Value Next()
      {
         using (var assistant = new ParameterAssistant(new Arguments(new NullBlock(), null, parameters)))
         using (var popper = new RegionPopper(region, "Seq-comprehension"))
         {
            popper.Push();
            if (arrayBlock == null)
               return new Nil();

            assistant.ArrayParameters();
            if (ifBlock != null)
            {
               if (innerComprehension == null)
               {
                  var value = getNext();
                  if (value.IsNil)
                     return value;

                  assistant.SetParameterValues(value, index.ToString(), index);
                  if (ifBlock.Evaluate().IsTrue)
                  {
                     value = block.Evaluate();
                     if (!value.IsNil)
                        return value;
                  }
               }
               else
               {
                  var value = getNext();
                  if (value.IsNil)
                     return value;

                  assistant.SetParameterValues(value, index.ToString(), index);
                  if (ifBlock.Evaluate().IsTrue)
                     return innerComprehension.Next();
               }
            }
            else
            {
               if (innerComprehension == null)
               {
                  var value = getNext();
                  if (value.IsNil)
                     return value;

                  assistant.SetParameterValues(value, index.ToString(), index);
                  value = block.Evaluate();
                  return value;
               }
               else
               {
                  var value = getNext();
                  if (value.IsNil)
                     return value;

                  assistant.SetParameterValues(value, index.ToString(), index);
                  return innerComprehension.Next();
               }
            }

            return null;
         }
      }

      void pushDownInnerComprehension(SequenceComprehension comprehension)
      {
         if (innerComprehension != null)
            innerComprehension.pushDownInnerComprehension(comprehension);
         else
         {
            innerComprehension = comprehension;
            innerComprehension.block = block;
            block = null;
         }
      }

      public Value Map()
      {
         var comprehension = new SequenceComprehension(block, Arguments.Parameters)
         {
            ArrayBlock = Arguments.Executable
         };
         pushDownInnerComprehension(comprehension);
         block = null;
         return this;
      }

      public Value If()
      {
         setIf(Arguments.Block);
         return this;
      }

      void setIf(Block block)
      {
         if (innerComprehension == null)
            ifBlock = block;
         else
            innerComprehension.setIf(block);
      }

      public ISequenceSource Copy() => (ISequenceSource)Clone();

      public Value Reset()
      {
         index = 0;
         arrayValue = null;
         sequence = null;
         return null;
      }

      public int Limit { get; set; }

      public Array Array => Array.ArrayFromSequence(this);
   }
}
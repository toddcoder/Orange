using System.Collections.Generic;
using Orange.Library.Managers;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Sequence : Value, ISequenceSource
   {
      public abstract class Item
      {
         public abstract Value Process(Array.IterItem item, int length);

         public abstract SignalType Signal { get; }

         public virtual bool IsBreaking => false;
      }

      public class MapItem : Item
      {
         ParameterBlock parameterBlock;
         SignalType signal;

         public MapItem(ParameterBlock parameterBlock) => this.parameterBlock = parameterBlock;

         public override Value Process(Array.IterItem item, int length)
         {
            signal = SignalType.None;
            using (var assistant = new ParameterAssistant(parameterBlock))
            {
               var block = assistant.Block();
               assistant.ArrayParameters();
               assistant.SetParameterValues(item);
               var value = block.Evaluate();
               signal = Signal();
               return value;
            }
         }

         public override SignalType Signal => signal;

         public override string ToString() => "-> " + parameterBlock;
      }

      public class IfItem : Item
      {
         ParameterBlock parameterBlock;
         SignalType signal;

         public IfItem(ParameterBlock parameterBlock) => this.parameterBlock = parameterBlock;

         public override Value Process(Array.IterItem item, int length)
         {
            signal = SignalType.None;
            using (var assistant = new ParameterAssistant(parameterBlock))
            {
               var block = assistant.Block();
               assistant.ArrayParameters();
               assistant.SetParameterValues(item);
               var value = block.Evaluate().IsTrue ? item.Value : null;
               signal = Signal();
               return value;
            }
         }

         public override SignalType Signal => signal;

         public override string ToString() => "-? " + parameterBlock;
      }

      public class UnlessItem : Item
      {
         ParameterBlock parameterBlock;
         SignalType signal;

         public UnlessItem(ParameterBlock parameterBlock) => this.parameterBlock = parameterBlock;

         public override Value Process(Array.IterItem item, int length)
         {
            signal = SignalType.None;
            using (var assistant = new ParameterAssistant(parameterBlock))
            {
               var block = assistant.Block();
               assistant.ArrayParameters();
               assistant.SetParameterValues(item);
               var value = block.Evaluate().IsTrue ? null : item.Value;
               signal = Signal();
               return value;
            }
         }

         public override SignalType Signal => signal;

         public override string ToString() => "-! " + parameterBlock;

         public override bool IsBreaking => true;
      }

      public class TakeItem : Item
      {
         int limit;

         public TakeItem(int limit) => this.limit = limit;

         public override Value Process(Array.IterItem item, int length) => length < limit ? item.Value : null;

         public override SignalType Signal => SignalType.None;

         public override bool IsBreaking => true;

         public override string ToString() => $@"\\ {limit}";
      }

      public class TakeBlockItem : Item
      {
         ParameterBlock parameterBlock;
         SignalType signal;

         public TakeBlockItem(ParameterBlock parameterBlock) => this.parameterBlock = parameterBlock;

         public override Value Process(Array.IterItem item, int length)
         {
            signal = SignalType.None;
            using (var assistant = new ParameterAssistant(parameterBlock))
            {
               var block = assistant.Block();
               assistant.ArrayParameters();
               assistant.SetParameterValues(item);
               var value = block.Evaluate().IsTrue ? item.Value : null;
               signal = Signal();
               return value;
            }
         }

         public override SignalType Signal => signal;

         public override string ToString() => $"@\\ {parameterBlock}";

         public override bool IsBreaking => true;
      }

      ISequenceSource source;
      List<Item> items;
      int index;
      Region region;

      public Sequence(ISequenceSource source, Region region = null)
      {
         this.source = source;
         this.region = Region.CopyCurrent(region);
         items = new List<Item>();
         index = -1;
      }

      public override int Compare(Value value) => 0;

      public override string Text
      {
         get { return AlternateValue("str").Text; }
         set { }
      }

      public override double Number
      {
         get { return AlternateValue("num").Number; }
         set { }
      }

      public override ValueType Type => ValueType.Sequence;

      public override bool IsTrue => true;

      public override Value Clone() => new Sequence(source.Copy(), region.Clone());

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "map", v => ((Sequence)v).Map());
         manager.RegisterMessage(this, "if", v => ((Sequence)v).If());
         manager.RegisterMessage(this, "unless", v => ((Sequence)v).Unless());
         manager.RegisterMessage(this, "take", v => ((Sequence)v).Take());
         manager.RegisterMessage(this, "next", v => ((Sequence)v).Next());
         manager.RegisterMessage(this, "reset", v => ((Sequence)v).Reset());
         manager.RegisterMessage(this, "arr", v => ((Sequence)v).Arr());
      }

      public Value Map()
      {
         items.Add(new MapItem(new ParameterBlock(Arguments.Parameters, Arguments.Executable, Arguments.Splatting)));
         return this;
      }

      public Value If()
      {
         items.Add(new IfItem(new ParameterBlock(Arguments.Parameters, Arguments.Executable, Arguments.Splatting)));
         return this;
      }

      public Value Unless()
      {
         items.Add(new UnlessItem(new ParameterBlock(Arguments.Parameters, Arguments.Executable, Arguments.Splatting)));
         return this;
      }

      public Value Take()
      {
         if (Arguments[0].IsEmpty)
            items.Add(new TakeBlockItem(new ParameterBlock(Arguments.Parameters, Arguments.Executable, Arguments.Splatting)));
         else
            items.Add(new TakeItem((int)Arguments[0].Number));
         return this;
      }

      public Value Next()
      {
         using (var popper = new RegionPopper(region, "next"))
         {
            popper.Push();
            index++;
            for (var i = 0; i < MAX_ARRAY; i++)
            {
               var value = source.Next();
               if (value.Type == ValueType.Nil)
                  return value;

               var skip = false;
               foreach (var item in items)
               {
                  var iterItem = new Array.IterItem
                  {
                     Value = value,
                     Key = index.ToString(),
                     Index = index
                  };
                  value = item.Process(iterItem, index);
                  if (value == null && item.IsBreaking)
                     return new Nil();

                  if (value == null || value.Type == ValueType.Nil || item.Signal == SignalType.Continuing)
                  {
                     skip = true;
                     break;
                  }

                  if (item.Signal == SignalType.Breaking)
                     return new Nil();
               }

               if (!skip)
                  return value;
            }

            return new Nil();
         }
      }

      public ISequenceSource Copy() => (ISequenceSource)Clone();

      public Value Reset()
      {
         index = -1;
         return source.Reset();
      }

      public int Limit => source.Limit;

      public Array Array => getArray();

      Array getArray()
      {
         using (var popper = new RegionPopper(region, "seq-get-array"))
         {
            popper.Push();
            var array = new Array();
            var i = 0;
            source.Reset();
            while (array.Length < source.Limit && i <= MAX_ARRAY)
            {
               var itemIndex = i++;
               var value = source.Next();
               if (value.Type == ValueType.Nil)
                  return array;

               var skip = false;
               foreach (var item in items)
               {
                  var iterItem = new Array.IterItem
                  {
                     Value = value,
                     Key = itemIndex.ToString(),
                     Index = itemIndex
                  };
                  value = item.Process(iterItem, array.Length);
                  if (value == null && item.IsBreaking)
                     return array;

                  if (value == null || value.Type == ValueType.Nil || item.Signal == SignalType.Continuing)
                  {
                     skip = true;
                     break;
                  }

                  if (item.Signal == SignalType.Breaking)
                     return array;
               }

               if (!skip)
                  array.Add(value);
            }

            return array;
         }
      }

      public override Value AlternateValue(string message) => getArray();

      public override Value SourceArray => getArray();

      public override bool IsArray => true;

      public override string ToString() => source.ToString();

      public Value Arr() => getArray();
   }
}
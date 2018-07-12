using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using static Orange.Library.Compiler;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class Indexer : Variable
   {
      const string LOCATION = "Indexer";

      Array array;
      Block keyBlock;
      bool isInteger;
      Array intKeys;

      public Indexer(Array array, Block keyBlock)
         : base(VAR_ANONYMOUS + CompilerState.ObjectID())
      {
         this.array = array;
         this.keyBlock = keyBlock;
         isInteger = false;
      }

      public Indexer(Array array, Array intKeys)
         : base(VAR_ANONYMOUS + CompilerState.ObjectID())
      {
         this.array = array;
         this.intKeys = intKeys;
         isInteger = true;
      }

      public override Value Value
      {
         get => isInteger ? array[getIndexes()] : array[getKeys()];
         set
         {
            if (isInteger)
               array[getIndexes()] = value;
            else
               array[getKeys()] = value;
         }
      }

      public override Value AlternateValue(string message) => Value;

      void setKeysLength()
      {
         var length = array.Length;
         foreach (var wrapping in keyBlock.OfType<IWrapping>())
         {
            wrapping.SetLength(length);
            wrapping.IsSlice = true;
         }
      }

      string[] getKeys()
      {
         setKeysLength();
         var keyArray = keyBlock.ToActualArguments();
         var keyValues = new List<string>();
         foreach (var value in keyArray.Values)
            switch (value.Type)
            {
               case ValueType.Array:
                  keyValues.AddRange(((Array)value).Select(item => item.Value.Text));
                  break;
               default:
                  keyValues.Add(value.Text);
                  break;
            }

         return keyValues.ToArray();
      }

      int[] getIndexes() => intKeys.Values.Select(v => (int)v.Number).ToArray();

      protected override void registerMessages(MessageManager manager)
      {
         base.registerMessages(manager);
         manager.RegisterMessage(this, "smap", v => ((Indexer)v).SelfMap());
         manager.RegisterMessage(this, "remove", v => ((Indexer)v).Remove());
         manager.RegisterMessage(this, "fill", v => ((Indexer)v).Fill());
         manager.RegisterMessage(this, "keys", v => ((Indexer)v).Keys());
         manager.RegisterMessage(this, "indexes", v => ((Indexer)v).Indexes());
         manager.RegisterMessage(this, "insert", v => ((Indexer)v).Insert());
         manager.RegisterMessage(this, "swap", v => ((Indexer)v).Swap());
      }

      public Value Swap() => isInteger ? swapIndexes() : swapKeys();

      Value swapKeys()
      {
         var swapKeys = getKeys();
         Reject(swapKeys.Length < 2, LOCATION, "Must have at least 2 keys");
         var leftKey = swapKeys[0];
         var rightKey = swapKeys[1];
         var left = array[leftKey];
         var right = array[rightKey];
         array[leftKey] = right;
         array[rightKey] = left;
         return array;
      }

      Value swapIndexes()
      {
         var swapIndexes = getIndexes();
         Reject(swapIndexes.Length < 2, LOCATION, "Must have at least 2 keys");
         var leftIndex = swapIndexes[0];
         var rightIndex = swapIndexes[1];
         var left = array[leftIndex];
         var right = array[rightIndex];
         array[leftIndex] = right;
         array[rightIndex] = left;
         return array;
      }

      public Value Insert() => isInteger ? insertIndex() : insertKey();

      Value insertKey()
      {
         var value = Arguments[0];
         var insertKeys = getKeys();
         foreach (var key in insertKeys)
            array.Insert(key, value);

         return array;
      }

      Value insertIndex()
      {
         var value = Arguments[0];
         var insertIndexes = getIndexes();
         foreach (var index in insertIndexes)
            array.Insert(index, value);

         return array;
      }

      public Value Keys() => new Array(getKeys());

      public Value Indexes() => keyBlock.ToActualArguments();

      public Value Fill()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            Value value;
            if (block == null)
            {
               value = Arguments[0];
               foreach (var key in getKeys())
                  array[key] = value.Clone();

               return array;
            }

            assistant.ArrayParameters();
            var index = 0;
            foreach (var key in getKeys())
            {
               assistant.SetParameterValues(array[key], key, index++);
               value = block.Evaluate();
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;

               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return null;
               }

               if (value != null)
                  array[key] = value;
            }

            return array;
         }
      }

      public override Value DefaultTo() => isInteger ? defaultToKeys() : defaultToIndexes();

      Value defaultToKeys()
      {
         var value = Arguments[0];
         var result = new Array();
         foreach (var key in getKeys())
            if (array.ContainsKey(key))
               result[key] = array[key];
            else
            {
               array[key] = value;
               result[key] = value;
            }

         return result.Length == 1 ? result[0] : result;
      }

      Value defaultToIndexes()
      {
         var value = Arguments[0];
         var result = new Array();
         foreach (var index in getIndexes())
            if (array.ContainsIndex(index))
               result[index] = array[index];
            else
            {
               array[index] = value;
               result[index] = value;
            }

         return result.Length == 1 ? result[0] : result;
      }

      public Value Remove() => isInteger ? removeIndexes() : removeKeys();

      Value removeKeys()
      {
         var keysToDelete = getKeys();
         foreach (var key in keysToDelete)
            array.Remove(key);

         return array;
      }

      Value removeIndexes()
      {
         var indexesToDelete = getIndexes();
         foreach (var index in indexesToDelete)
            array.Remove(index);

         return array;
      }

      public override string ContainerType => ValueType.Indexer.ToString();

      public Value SelfMap() => isInteger ? selfMapIndexes() : selfMapKeys();

      Value selfMapKeys()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var changes = new Array();

            foreach (var key in getKeys())
            {
               var value = array[key];
               var index = array.GetIndex(key);
               assistant.SetParameterValues(value, key, index);
               value = block.Evaluate();
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;

               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return null;
               }

               if (value.Type == ValueType.Nil)
                  continue;

               if (value is KeyedValue keyedValue)
                  changes[keyedValue.Key] = keyedValue.Value;
               else
                  changes[key] = value;
            }

            if (changes.Length == 0)
               return this;

            foreach (var item in changes)
               array[item.Key] = item.Value;

            return this;
         }
      }

      Value selfMapIndexes()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var changes = new Array();

            foreach (var index in getIndexes())
            {
               var value = array[index];
               var key = array.GetKey(index);
               assistant.SetParameterValues(value, key, index);
               value = block.Evaluate();
               var signal = ParameterAssistant.Signal();
               if (signal == ParameterAssistant.SignalType.Breaking)
                  break;

               switch (signal)
               {
                  case ParameterAssistant.SignalType.Continuing:
                     continue;
                  case ParameterAssistant.SignalType.ReturningNull:
                     return null;
               }

               if (value.Type == ValueType.Nil)
                  continue;

               if (value is KeyedValue keyedValue)
                  changes[keyedValue.Key] = keyedValue.Value;
               else
                  changes[key] = value;
            }

            if (changes.Length == 0)
               return this;

            foreach (var item in changes)
               array[item.Key] = item.Value;

            return this;
         }
      }

      public override Value MessageTarget(string message) => this;

      public override bool IsIndexer => true;

      public override Value Resolve() => this;

      public override string ToString() => Value.ToString();
   }
}
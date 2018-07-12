using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class KeyIndexer : BaseIndexer<string>
   {
      Array keyArray;

      public KeyIndexer(Array array, Array keyArray)
         : base(array) => this.keyArray = keyArray;

      protected override void registerMessages(MessageManager manager)
      {
         base.registerMessages(manager);
         manager.RegisterMessage(this, "smap", v => ((KeyIndexer)v).SelfMap());
         manager.RegisterMessage(this, "remove", v => ((KeyIndexer)v).Remove());
         manager.RegisterMessage(this, "fill", v => ((KeyIndexer)v).Fill());
         manager.RegisterMessage(this, "insert", v => ((KeyIndexer)v).Insert());
         manager.RegisterMessage(this, "swap", v => ((KeyIndexer)v).Swap());
         manager.RegisterMessage(this, "get", v => ((KeyIndexer)v).Get());
      }

      protected override string[] getIndicators()
      {
         setLength();
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

      protected override Value getSlice(string[] indicators) => array[getIndicators()];

      protected override void setSlice(string[] indicators, Value value) => array[getIndicators()] = value;

      protected override void setLength()
      {
         var length = array.Length;
         foreach (var wrapping in keyArray.OfType<IWrapping>())
         {
            wrapping.SetLength(length);
            wrapping.IsSlice = true;
         }
      }

      public override Value SelfMap()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var changes = new Array();

            foreach (var key in getIndicators())
            {
               var value = array[key];
               var index = array.GetIndex(key);
               assistant.SetParameterValues(value, key, index);
               value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  break;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
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

      public override Value Remove()
      {
         var keysToDelete = getIndicators();
         foreach (var key in keysToDelete)
            array.Remove(key);

         return array;
      }

      public override Value Fill()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            Value value;
            if (block != null)
            {
               assistant.ArrayParameters();
               var index = 0;
               foreach (var key in getIndicators())
               {
                  assistant.SetParameterValues(array[key], key, index++);
                  value = block.Evaluate();
                  var signal = Signal();
                  if (signal == Breaking)
                     break;

                  switch (signal)
                  {
                     case Continuing:
                        continue;
                     case ReturningNull:
                        return null;
                  }

                  if (value != null)
                     array[key] = value;
               }

               return array;
            }

            value = Arguments[0];
            foreach (var key in getIndicators())
               array[key] = value.Clone();

            return array;
         }
      }

      public override Value Insert()
      {
         var value = Arguments[0];
         var insertKeys = getIndicators();
         foreach (var key in insertKeys)
            array.Insert(key, value);

         return array;
      }

      public override Value Swap()
      {
         var swapKeys = getIndicators();
         Reject(swapKeys.Length < 2, "Key indexer", "Must have at least 2 keys");
         var leftKey = swapKeys[0];
         var rightKey = swapKeys[1];
         var left = array[leftKey];
         var right = array[rightKey];
         array[leftKey] = right;
         array[rightKey] = left;
         return array;
      }

      public override Value Get()
      {
         var value = Arguments[0];
         var defaultValue = GetDefault.Create(value);
         var keys = getIndicators();
         Value get;
         switch (keys.Length)
         {
            case 0:
               return new Array();
            case 1:
               var key = keys[0];
               if (array.ContainsKey(key))
                  return array[key];

               get = defaultValue.Value();
               array[key] = get;
               return get;
            default:
               var newArray = new Array();
               foreach (var keyToFind in keys)
                  if (array.ContainsKey(keyToFind))
                     newArray.Add(array[keyToFind]);
                  else
                  {
                     get = defaultValue.Value();
                     array[keyToFind] = get;
                     newArray.Add(get);
                  }

               return newArray;
         }
      }

      public override Value DefaultTo()
      {
         var value = Arguments[0];
         var result = new Array();
         foreach (var key in getIndicators())
            if (array.ContainsKey(key))
               result[key] = array[key];
            else
            {
               array[key] = value;
               result[key] = value;
            }

         return result.Length == 1 ? result[0] : result;
      }

      public override Value MessageTarget(string message)
      {
         switch (message)
         {
            case "smap":
            case "remove":
            case "fill":
            case "insert":
            case "swap":
            case "get":
               return this;
            default:
               return Value;
         }
      }
   }
}
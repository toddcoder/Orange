using System.Linq;
using Orange.Library.Managers;
using static System.Array;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;

namespace Orange.Library.Values
{
   public class IndexIndexer : BaseIndexer<int>
   {
      Array indexes;

      public IndexIndexer(Array array, Array indexes)
         : base(array) => this.indexes = indexes;

      protected override int[] getIndicators() => indexes.Values.Select(v => (int)v.Number).ToArray();

      protected override Value getSlice(int[] indicators) => array[indicators];

      protected override void setSlice(int[] indicators, Value value) => array[indicators] = value;

      protected override void setLength() { }

      protected override void registerMessages(MessageManager manager)
      {
         base.registerMessages(manager);
         manager.RegisterMessage(this, "smap", v => ((IndexIndexer)v).SelfMap());
         manager.RegisterMessage(this, "remove", v => ((IndexIndexer)v).Remove());
         manager.RegisterMessage(this, "fill", v => ((IndexIndexer)v).Fill());
         manager.RegisterMessage(this, "insert", v => ((IndexIndexer)v).Insert());
         manager.RegisterMessage(this, "swap", v => ((IndexIndexer)v).Swap());
         manager.RegisterMessage(this, "get", v => ((IndexIndexer)v).Get());
         manager.RegisterMessage(this, "copy", v => ((IndexIndexer)v).Copy());
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

            foreach (var index in getIndicators())
            {
               var value = array[index];
               var key = array.GetKey(index);
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
         var indexesToDelete = getIndicators();
         Sort(indexesToDelete, (x, y) => y.CompareTo(x));
         foreach (var index in indexesToDelete)
            array.Remove(index);

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
               foreach (var index in getIndicators())
               {
                  var key = array.GetKey(index);
                  assistant.SetParameterValues(array[index], key, index);
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
                     array[index] = value;
               }

               return array;
            }

            value = Arguments[0];
            foreach (var index in getIndicators())
               array[index] = value.Clone();

            return array;
         }
      }

      public override Value Insert()
      {
         var value = Arguments[0];
         var insertIndexes = getIndicators();
         Sort(insertIndexes, (x, y) => y.CompareTo(x));
         foreach (var index in insertIndexes)
            array.Insert(index, value);

         return array;
      }

      public override Value Swap()
      {
         var swapIndexes = getIndicators();
         Reject(swapIndexes.Length < 2, "Index indexer", "Must have at least 2 indexes");
         var leftIndex = swapIndexes[0];
         var rightIndex = swapIndexes[1];
         var left = array[leftIndex];
         var right = array[rightIndex];
         array[leftIndex] = right;
         array[rightIndex] = left;
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
               if (array.ContainsIndex(key))
                  return array[key];

               get = defaultValue.Value();
               array[key] = get;
               return get;
            default:
               var newArray = new Array();
               foreach (var keyToFind in keys)
                  if (array.ContainsIndex(keyToFind))
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
         foreach (var index in getIndicators())
            if (array.ContainsIndex(index))
               result[index] = array[index];
            else
            {
               array[index] = value;
               result[index] = value;
            }

         return result.Length == 1 ? result[0] : result;
      }

      public Value Copy()
      {
         var target = array.Copy();
         var source = Arguments.AsArray();
         if (source == null)
            return this;

         var sourceIndex = 0;
         foreach (var index in getIndicators())
            target[index] = source[sourceIndex++];

         return target;
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
            case "copy":
               return this;
            default:
               return Value;
         }
      }
   }
}
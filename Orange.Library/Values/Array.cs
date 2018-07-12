using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orange.Library.Managers;
using Orange.Library.Messages;
using Orange.Library.Parsers;
using Orange.Library.Verbs;
using Standard.Types.Arrays;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using ArrayBase = Standard.Types.Collections.AutoHash<string, Orange.Library.Values.Value>;
using IndexBase = Standard.Types.Collections.AutoHash<int, string>;
using KeyValueBase = System.Collections.Generic.KeyValuePair<string, Orange.Library.Values.Value>;
using static System.Math;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;
using static Orange.Library.Values.NSIterator;
using static Orange.Library.Values.Null;
using static Standard.Types.Lambdas.LambdaFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class Array : Value, IEnumerable<Array.IterItem>, IComparer<double>, IComparer<string>, IRepeatable,
      IMessageHandler, IComparer<Value>, INSGeneratorSource
   {
      public class Pusher
      {
         Array array;

         public Pusher() => array = new Array();

         public void Add(Value value) => array.Add(value);

         public void AddRange(IEnumerable<Value> values)
         {
            foreach (var value in values)
               Add(value);
         }

         public void Push(Value value)
         {
            if (array.Length == 0)
               array.Add(value);
            else
            {
               var index = array.Length - 1;
               var last = array[index];
               if (last is Array innerArray)
                  innerArray.Add(value);
               else
                  array[index] = new Array { last, value };
            }
         }

         public void PushRange(IEnumerable<Value> values)
         {
            foreach (var value in values)
               Push(value);
         }

         public Array Array => array;
      }

      public class ArrayEnumerator : IEnumerator<IterItem>
      {
         Array array;
         int index;

         public ArrayEnumerator(Array array)
         {
            this.array = array;
            index = -1;
         }

         public void Dispose() { }

         public bool MoveNext()
         {
            if (State.ExitSignal)
            {
               State.ExitSignal = false;
               return false;
            }

            if (State.SkipSignal)
               State.SkipSignal = false;
            if (State.ReturnSignal)
               return false;

            index++;
            var moveNext = index < array.Length;
            return moveNext;
         }

         public void Reset() => index = -1;

         public IterItem Current => array.GetArrayItem(index);

         object IEnumerator.Current => Current;
      }

      public class IterItem
      {
         public IterItem()
         {
            Key = "";
            Index = -1;
            Value = new String("");
         }

         public IterItem(KeyValueBase item, int index = -1)
         {
            Key = item.Key;
            Index = index;
            Value = item.Value;
         }

         public string Key { get; set; }

         public int Index { get; set; }

         public Value Value { get; set; }

         public override string ToString() => $"{Key} {Index} {Value}";
      }

      public enum CompareType
      {
         Any,
         All,
         One,
         None
      }

      const string LOCATION = "Array";

      public static Array SliceRange(int start, int stop, int length, bool inside, int increment = 1) =>
         inside ? SliceRangeInside(start, stop, length, increment) : SliceRangeOutside(start, stop, increment);

      public static Array SliceRangeInside(int start, int stop, int length, int increment = 1)
      {
         increment = Abs(increment);
         if (stop < 0)
            stop = WrapIndex(stop, length, true);
         var array = new Array();
         if (start <= stop)
            for (var i = start; i <= stop; i += increment)
            {
               var value = i;
               if (value < 0)
                  value = WrapIndex(value, length, true);
               array.Add(value);
            }
         else
            for (var i = start; i >= stop; i -= increment)
            {
               var value = i;
               if (value < 0)
                  value = WrapIndex(value, length, true);
               array.Add(value);
            }

         return array;
      }

      public static Array SliceRangeOutside(int start, int stop, int increment = 1)
      {
         increment = Abs(increment);
         var array = new Array();
         if (start <= stop)
            for (var i = start; i <= stop; i += increment)
            {
               var value = i;
               array.Add(value);
            }
         else
            for (var i = start; i >= stop; i -= increment)
            {
               var value = i;
               array.Add(value);
            }

         return array;
      }

      public static Array Concatenate(Array x, Array y)
      {
         var array = new Array();
         if (x == null || y == null)
            return array;

         foreach (var item in x)
            array.Add(item.Value);
         foreach (var item in y)
            array.Add(item.Value);

         return (Array)array.Flatten();
      }

      public static Array ConcatenateValue(Array sourceArray, Value value)
      {
         var array = new Array();
         foreach (var item in sourceArray)
            array.Add(item.Value);

         array.Add(value);
         return array;
      }

      protected static ArrayBase createArrayBase()
      {
         return new ArrayBase { Default = DefaultType.Lambda, DefaultLambda = k => NullValue };
      }

      protected static IndexBase createIndexBase()
      {
         return new IndexBase { Default = DefaultType.Lambda, DefaultLambda = i => "" };
      }

      public static Array Repeat(Value value, int count)
      {
         var array = new Array();
         for (var i = 0; i < count; i++)
            array.Add(value.Clone());

         return array;
      }

      public static Array ArrayFromSequence(ISequenceSource source)
      {
         source.Reset();
         var value = source.Next();
         var array = new Array();
         while (value != null && !value.IsNil && array.Length <= MAX_ARRAY)
         {
            array.Add(value);
            value = source.Next();
         }

         return array;
      }

      public static List ArrayToList(Array array)
      {
         switch (array.Length)
         {
            case 0:
               return Library.Values.List.Empty;
            case 1:
               return new List(array[0]);
         }

         var list = new List(array[array.Length - 1]);
         for (var i = array.Length - 2; i > -1; i--)
            list = Library.Values.List.Cons(array[i], list);

         return list;
      }

      public static bool IsAutoAssignedKey(string key) => key.IsMatch("^ '__$key' /d+ $");

      protected ArrayBase array;
      protected IndexBase indexes;
      protected bool suspendIndexUpdate;
      protected bool reconstitueArray;
      protected Lambda newValueLambda;
      protected int betweening;
      protected bool exclusive;
      protected bool maybe;

      public Array()
      {
         array = createArrayBase();
         indexes = createIndexBase();
         suspendIndexUpdate = false;
         reconstitueArray = false;
         maybe = false;
      }

      public Array(string[] array)
      {
         this.array = createArrayBase();
         indexes = createIndexBase();
         suspendIndexUpdate = true;
         foreach (var value in array)
            basicAdd(ConvertIfNumeric(value));

         suspendIndexUpdate = false;
         reconstitueArray = false;
         updateIndexes();
         maybe = false;
      }

      public Array(IEnumerable<Value> values)
      {
         array = createArrayBase();
         indexes = createIndexBase();
         suspendIndexUpdate = true;
         foreach (var value in values)
            basicAdd(value);

         suspendIndexUpdate = false;
         reconstitueArray = true;
         updateIndexes();
         maybe = false;
      }

      public Array(IterItem[] items)
      {
         array = createArrayBase();
         indexes = createIndexBase();
         var index = 0;
         foreach (var item in items)
         {
            array[item.Key] = item.Value;
            indexes[index++] = item.Key;
         }

         suspendIndexUpdate = false;
         reconstitueArray = false;
         updateIndexes();
         maybe = false;
      }

      public Array(IOrderedEnumerable<KeyValuePair<string, Value>> sortedArray)
      {
         array = createArrayBase();
         indexes = createIndexBase();
         foreach (var item in sortedArray)
            this[item.Key] = item.Value;

         suspendIndexUpdate = false;
         reconstitueArray = false;
         updateIndexes();
         maybe = false;
      }

      public Array(ArrayBase hash)
      {
         array = createArrayBase();
         indexes = createIndexBase();
         foreach (var item in hash)
            this[item.Key] = item.Value;

         suspendIndexUpdate = false;
         reconstitueArray = false;
         updateIndexes();
         maybe = false;
      }

      void updateIndexes()
      {
         if (suspendIndexUpdate)
            return;

         indexes.Clear();
         foreach (var item in array)
            updateIndex(item.Key);
      }

      void updateIndex(string key) => indexes[indexes.Count] = key;

      public virtual Value this[string key]
      {
         get
         {
            if (key == null)
               key = "";
            if (maybe)
            {
               if (array.ContainsKey(key))
                  return new Some(array[key]);

               return new None();
            }

            var value = array[key];
            if (array.AutoAddDefault)
               updateIndexes();
            return value;
         }
         set
         {
            if (key == null)
               key = "";
            if (value.ID == id)
               return;

            if (value.IsNil)
               array.Remove(key);
            else
               array[key] = value.AssignmentValue();
            updateIndexes();
         }
      }

      public virtual Value this[string[] keys]
      {
         get
         {
            if (keys.Length == 1)
               return this[keys[0]];

            var slice = new Array();
            foreach (var value in keys.Select(key => this[key]))
               slice.Add(value);

            return slice;
         }
         set
         {
            if (value.ID == id)
               return;

            if (value.IsArray)
            {
               suspendIndexUpdate = true;
               var sourceArray = (Array)value.SourceArray;
               var targetLength = keys.Length;
               var sourceLength = sourceArray.Length;
               if (targetLength == 1)
               {
                  array[keys[0]] = value;
                  suspendIndexUpdate = false;
                  updateIndexes();
                  return;
               }

               var minLength = Math.Min(targetLength, sourceLength);
               var index = -1;
               var lastI = -1;
               for (var i = 0; i < minLength; i++)
               {
                  var key = keys[i];
                  index = GetIndex(key);
                  this[key] = sourceArray[i];
                  lastI = i;
               }

               if (sourceLength > minLength)
               {
                  var lastValue = this[index];
                  var subArray = new Array { lastValue };
                  for (var i = lastI + 1; i < sourceLength; i++)
                     subArray.Add(sourceArray[i]);

                  this[index] = subArray;
               }

               suspendIndexUpdate = false;
               updateIndexes();
            }
            else if (value.IsNil)
               foreach (var key in keys)
                  Remove(key);
            else
               foreach (var key in keys)
                  this[key] = value;
         }
      }

      public virtual Value this[int index]
      {
         get
         {
            if (index < 0)
               index = WrapIndex(index, array.Count, false);
            return array[indexes[index]];
         }
         set
         {
            if (value.ID == id)
               return;

            if (value.IsNil)
            {
               Remove(index);
               return;
            }

            suspendIndexUpdate = true;
            string key;
            Value baseValue;
            var hashKey = value as KeyedValue;
            if (hashKey == null)
            {
               key = getKey();
               baseValue = value.AssignmentValue();
            }
            else
            {
               key = hashKey.Key;
               baseValue = hashKey.Value;
            }

            if (index < 0)
               index = WrapIndex(index, array.Count, false);
            if (index == indexes.Count)
            {
               array[key] = baseValue;
               indexes[index] = key;
            }
            else if (index > indexes.Count)
            {
               var defaultValue = ((Variable)Default()).Value;
               for (var i = indexes.Count; i < index; i++)
                  Add(defaultValue);

               Add(baseValue);
            }
            else
               array[indexes[index]] = baseValue;

            suspendIndexUpdate = false;
            updateIndexes();
         }
      }

      public virtual Value this[int[] keys]
      {
         get
         {
            if (keys.Length == 1)
               return this[keys[0]];

            var slice = new Array();
            foreach (var value in keys.Select(k => this[k]))
               slice.Add(value);

            return slice;
         }
         set
         {
            if (value.ID == id)
               return;

            if (value.IsArray)
            {
               suspendIndexUpdate = true;
               var sourceArray = (Array)value.SourceArray;
               var targetLength = keys.Length;
               var sourceLength = sourceArray.Length;
               if (targetLength == 1)
               {
                  var key = GetKey(keys[0]);
                  array[key] = value;
                  suspendIndexUpdate = false;
                  updateIndexes();
                  return;
               }

               var minLength = Math.Min(targetLength, sourceLength);
               var index = -1;
               var lastI = -1;
               for (var i = 0; i < minLength; i++)
               {
                  var key = keys[i];
                  index = key;
                  this[key] = sourceArray[i];
                  lastI = i;
               }

               if (sourceLength > minLength)
               {
                  var lastValue = this[index];
                  var subArray = new Array
                  {
                     lastValue
                  };
                  for (var i = lastI + 1; i < sourceLength; i++)
                     subArray.Add(sourceArray[i]);

                  this[index] = subArray;
               }

               suspendIndexUpdate = false;
               updateIndexes();
            }
            else if (keys.Length == 1)
               this[keys[0]] = value;
            else
            {
               suspendIndexUpdate = true;
               var index1 = keys[0];
               System.Array.Sort(keys, (x, y) => y.CompareTo(x));
               foreach (var key in keys)
                  Remove(key);

               Insert(index1, value);
               suspendIndexUpdate = false;
               updateIndexes();
            }
         }
      }

      public Array Reverse()
      {
         var newArray = new Array();
         var stack = new Stack<IterItem>();

         foreach (var item in this)
            stack.Push(item);

         while (stack.Count > 0)
         {
            var item = stack.Pop();
            newArray[item.Key] = item.Value;
         }

         return newArray;
      }

      public virtual void Remove(string key)
      {
         if (!array.ContainsKey(key))
            return;

         suspendIndexUpdate = true;
         array.Remove(key);
         suspendIndexUpdate = false;
         updateIndexes();
         reconstitueArray = true;
      }

      public virtual void Remove(int index)
      {
         index = WrapIndex(index, array.Count, false);
         var key = indexes[index];
         Remove(key);
      }

      public virtual void Add(Value value) => basicAdd(value);

      void basicAdd(Value value)
      {
         Reject(value.Type == ValueType.System, LOCATION, "System variable can't be added!");
         string key;
         if (value is KeyedValue hashKey)
         {
            key = hashKey.Key;
            value = hashKey.Value;
         }
         else
            key = getKey();
         if (reconstitueArray)
         {
            ReconstitueArray();
            reconstitueArray = false;
         }
         this[key] = value.AssignmentValue();
      }

      public void ReconstitueArray()
      {
         var value = array.DefaultValue;
         var newArray = createArrayBase();
         newArray.DefaultValue = value;
         var newIndexes = createIndexBase();
         for (var i = 0; i < indexes.Count; i++)
         {
            var key = indexes[i];
            newArray.Add(key, array[key]);
            newIndexes[i] = key;
         }

         array = newArray;
         indexes = newIndexes;
      }

      public virtual Value Pop()
      {
         if (maybe && array.Count == 0)
            return new None();

         Reject(array.Count == 0, LOCATION, "Can't pop, array empty");
         var index = -1;
         var indexValue = Arguments?[0];
         if (!indexValue?.IsEmpty == true)
            index = (int)indexValue.Number;
         var value = this[index];
         Remove(index);
         return maybe ? new Some(value) : value;
      }

      public string[] Keys => array.KeyArray();

      public Value[] KeyValues => Keys.Select(k => (Value)new String(k)).ToArray();

      public int[] Indexes => indexes.KeyArray();

      public Value[] IndexValues => Indexes.Select(i => (Value)new Double(i)).ToArray();

      public Value[] Values => array.ValueArray();

      public IterItem[] Items => array.Select(i => new IterItem(i)).ToArray();

      public bool ContainsKey(string key) => array.ContainsKey(key);

      public bool ContainsIndex(int index)
      {
         index = WrapIndex(index, array.Count, false);
         var key = indexes[index];
         return ContainsKey(key);
      }

      public bool ContainsValue(Value value) => array.ContainsValue(value);

      public Array Copy()
      {
         var newArray = new Array();
         foreach (var item in array)
            newArray[item.Key] = item.Value.Clone();

         return newArray;
      }

      public Array CopyNoKeys()
      {
         var newArray = new Array();
         foreach (var item in this)
            newArray.Add(item.Value.Clone());

         return newArray;
      }

      public void Unshift(Value value)
      {
         var oldDefaultValue = array.DefaultValue;
         var newArray = createArrayBase();
         newArray.DefaultValue = oldDefaultValue;
         indexes.Clear();
         var key = getKey();
         newArray[key] = value.AssignmentValue();
         indexes[0] = key;

         foreach (var item in array)
         {
            newArray[item.Key] = item.Value.AssignmentValue();
            indexes[indexes.Count] = item.Key;
         }

         array = newArray;
      }

      public virtual Value Shift()
      {
         if (maybe && array.Count == 0)
            return new None();

         Assert(array.Count > 0, LOCATION, "Array is empty");

         var index = 0;
         var indexValue = Arguments?[0];
         if (!indexValue?.IsEmpty == true)
            index = (int)indexValue.Number;

         var result = this[index];

         var value = array.DefaultValue;
         var newArray = createArrayBase();
         newArray.DefaultValue = value;
         indexes.Clear();

         var firstFound = false;
         foreach (var item in array)
            if (firstFound)
            {
               newArray[item.Key] = item.Value.AssignmentValue();
               indexes[indexes.Count] = item.Key;
            }
            else
               firstFound = true;

         array = newArray;

         return maybe ? new Some(result) : result;
      }

      Value shiftOne()
      {
         if (Length == 0)
            return new Nil();

         var index = 0;
         var indexValue = Arguments?[0];
         if (!indexValue?.IsEmpty == true)
            index = (int)indexValue.Number;

         var result = this[index];

         var value = array.DefaultValue;
         var newArray = createArrayBase();
         newArray.DefaultValue = value;
         indexes.Clear();

         var firstFound = false;
         foreach (var item in array)
            if (firstFound)
            {
               newArray[item.Key] = item.Value.AssignmentValue();
               indexes[indexes.Count] = item.Key;
            }
            else
               firstFound = true;

         array = newArray;

         return result;
      }

      public virtual Value Clear()
      {
         array.Clear();
         indexes.Clear();
         return this;
      }

      public virtual void AddUnique(Value value)
      {
         if (!ContainsValue(value))
            Add(value);
      }

      public virtual int Length => array.Count;

      public IterItem GetArrayItem(string key)
      {
         if (Runtime.IsNumeric(key))
         {
            var index = key.ToInt();
            return GetArrayItem(index);
         }

         var item = this.FirstOrDefault(i => i.Key == key);
         return item;
      }

      public virtual IterItem GetArrayItem(int index)
      {
         var key = indexes[index];
         if (key == null)
            return null;

         var value = array[key];
         return new IterItem
         {
            Key = key,
            Index = index,
            Value = value
         };
      }

      static string getKey() => VAR_MANGLE + "key" + Compiler.CompilerState.ObjectID();

      public string GetKey(int index)
      {
         index = WrapIndex(index, array.Count, false);
         var item = GetArrayItem(index);
         return item.Key;
      }

      public int GetIndex(string key)
      {
         var item = GetArrayItem(key);
         if (item == null)
            return -1;

         return item.Index;
      }

      public virtual void Insert(int index, Value value)
      {
         index = WrapIndex(index, array.Count, false);
         var oldDefaultValue = array.DefaultValue;
         var newArray = createArrayBase();
         newArray.DefaultValue = oldDefaultValue;
         for (var i = 0; i < index; i++)
         {
            var key = indexes[i];
            newArray[key] = array[key];
         }

         if (value is KeyedValue keyedValue)
            newArray[keyedValue.Key] = keyedValue.Value;
         else
            newArray[getKey()] = value;
         for (var i = index; i < array.Count; i++)
         {
            var key = indexes[i];
            newArray[key] = array[key];
         }

         array = newArray;
         updateIndexes();
      }

      public virtual void Insert(string key, Value value)
      {
         var index = GetIndex(key);
         Insert(index, value);
      }

      public override int Compare(Value value)
      {
         var other = value.Resolve() as Array;
         if (other == null)
            return -1;

         var minLength = Math.Min(Length, other.Length);
         for (var i = 0; i < minLength; i++)
         {
            var compare = this[i].Compare(other[i]);
            if (compare != 0)
               return compare;
         }

         if (Length == other.Length)
            return 0;

         return Length > other.Length ? 1 : -1;
      }

      public override string Text
      {
         get
         {
            var connector = State.FieldSeparator.Text;
            var values = array.ValueArray();
            switch (values.Length)
            {
               case 0:
                  return "";
               case 1:
                  return values[0].Text;
               default:
                  var result = new StringBuilder();
                  var index = 0;
                  for (var i = 0; i < values.Length; i++)
                  {
                     var value = values[0];
                     if (value.Type == ValueType.Separator)
                        connector = value.Text;
                     else
                     {
                        result.Append(value.Text);
                        index = i + 1;
                        break;
                     }
                  }
                  for (var i = index; i < values.Length; i++)
                  {
                     var value = values[i];
                     if (value.Type == ValueType.Separator)
                     {
                        connector = value.Text;
                        continue;
                     }

                     result.Append(connector);
                     result.Append(value.Text);
                  }

                  return result.ToString();
            }
         }
         set { }
      }

      public override double Number
      {
         get { return array.Count; }
         set { }
      }

      public override ValueType Type => ValueType.Array;

      public override bool IsTrue => Length > 0;

      public override Value Clone() => Copy();

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterProperty(this, "item", v => ((Array)v).GetItem(), v => ((Array)v).SetItem());
         manager.RegisterMessage(this, "len", v => ((Array)v).Length);
         manager.RegisterMessage(this, "rev", v => ((Array)v).Reverse());
         manager.RegisterMessage(this, "del", v => ((Array)v).Delete());
         manager.RegisterMessage(this, "keys", v => new Array(((Array)v).KeyValues));
         manager.RegisterMessage(this, "indexes", v => new Array(((Array)v).IndexValues));
         manager.RegisterMessage(this, "values", v => new Array(((Array)v).Values));
         manager.RegisterMessage(this, "isKey", v => ((Array)v).ContainsKey());
         manager.RegisterMessage(this, "isIdx", v => ((Array)v).ContainsIndex());
         manager.RegisterMessage(this, "isVal", v => ((Array)v).ContainsValue());
         manager.RegisterMessage(this, "pop", v => ((Array)v).Pop());
         manager.RegisterMessage(this, "copy", v => ((Array)v).Copy());
         manager.RegisterMessage(this, "unshift", v => ((Array)v).Unshift());
         manager.RegisterMessage(this, "shift", v => ((Array)v).Shift());
         manager.RegisterMessage(this, "clear", v => ((Array)v).Clear());
         manager.RegisterMessage(this, "addUnique", v => ((Array)v).AddUnique());
         manager.RegisterMessage(this, "insertAt", v => ((Array)v).Insert());
         //manager.RegisterMessage(this, "unique", v => ((Array)v).Unique());
         manager.RegisterMessage(this, "set", v => ((Array)v).Set());
         manager.RegisterMessage(this, "each", v => ((Array)v).Each(), false);
         manager.RegisterMessage(this, "listify", v => ((Array)v).Listify());
         manager.RegisterMessage(this, "first", v => ((Array)v).First());
         manager.RegisterMessage(this, "last", v => ((Array)v).Last());
         manager.RegisterMessage(this, "notFirst", v => ((Array)v).NotTop());
         manager.RegisterMessage(this, "notLast", v => ((Array)v).NotBottom());
         manager.RegisterMessage(this, "mid", v => ((Array)v).Middle());
         manager.RegisterMessage(this, "notMid", v => ((Array)v).NotMiddle());
         manager.RegisterMessage(this, "byKeys", v => ((Array)v).ByKeys());
         manager.RegisterMessage(this, "byIndexes", v => ((Array)v).ByIndexes());
         manager.RegisterMessage(this, "sort", v => ((Array)v).sort(true));
         manager.RegisterMessage(this, "sortDesc", v => ((Array)v).sort(false));
         manager.RegisterMessage(this, "sortNum", v => ((Array)v).SortNumeric(true));
         manager.RegisterMessage(this, "sortNumDesc", v => ((Array)v).SortNumeric(false));
         manager.RegisterMessage(this, "sortDescNum", v => ((Array)v).SortNumeric(false));
         manager.RegisterMessage(this, "repeat", v => ((Array)v).Repeat());
         manager.RegisterMessage(this, "reducel", v => ((Array)v).ReduceL());
         manager.RegisterMessage(this, "reducer", v => ((Array)v).ReduceR());
         manager.RegisterMessage(this, "select", v => ((Array)v).Map(), false);
         manager.RegisterMessage(this, "push", v => ((Array)v).Push());
         manager.RegisterMessage(this, "add", v => ((Array)v).Push());
         manager.RegisterMessage(this, "end", v => ((Array)v).End());
         manager.RegisterMessage(this, "fill", v => ((Array)v).Fill());
         manager.RegisterMessage(this, "max", v => ((Array)v).Max());
         manager.RegisterMessage(this, "min", v => ((Array)v).Min());
         manager.RegisterMessage(this, "to", v => ((Array)v).Map());
         manager.RegisterMessage(this, "fmap", v => ((Array)v).FlatMap());
         //manager.RegisterMessage(this, "find", v => ((Array)v).Find());
         manager.RegisterMessage(this, "find", v => ((Array)v).FindIndex());
         manager.RegisterMessage(this, "count", v => ((Array)v).Count());
         manager.RegisterMessage(this, "join", v => ((Array)v).Join());
         manager.RegisterMessage(this, "zip", v => ((Array)v).Zip());
         manager.RegisterMessage(this, "zipDo", v => ((Array)v).ZipDo());
         manager.RegisterMessage(this, "isAny", v => ((Array)v).Any());
         manager.RegisterMessage(this, "isOne", v => ((Array)v).Any(c => c == 1));
         manager.RegisterMessage(this, "isAll", v => ((Array)v).All());
         manager.RegisterMessage(this, "isNone", v => ((Array)v).None());
         manager.RegisterMessage(this, "isEmpty", v => ((Array)v).Length == 0);
         manager.RegisterMessage(this, "sub", v => ((Array)v).RemoveWithKey());
         manager.RegisterMessage(this, "shuffle", v => ((Array)v).Shuffle());
         manager.RegisterMessage(this, "sample", v => ((Array)v).Sample());
         manager.RegisterMessage(this, "inv", v => ((Array)v).Invert());
         manager.RegisterMessage(this, "splice", v => ((Array)v).Splice());
         manager.RegisterMessage(this, "unfield", v => ((Array)v).Unfield());
         manager.RegisterMessage(this, "unrec", v => ((Array)v).Unrecord());
         manager.RegisterMessage(this, "removeWhere", v => ((Array)v).RemoveWhere());
         manager.RegisterMessage(this, "merge", v => ((Array)v).Merge());
         manager.RegisterMessage(this, "padder", v => ((Array)v).Padder());
         manager.RegisterProperty(this, "default", v => ((Array)v).GetDefault(), v => ((Array)v).SetDefault());
         manager.RegisterMessage(this, "indexOf", v => ((Array)v).IndexOf());
         manager.RegisterMessage(this, "key", v => ((Array)v).KeyOf());
         manager.RegisterMessage(this, "rot", v => ((Array)v).Rotate());
         manager.RegisterMessageCall("apply");
         manager.RegisterMessage(this, "apply", v => ((Array)v).Apply());
         manager.RegisterMessageCall("applyNot");
         manager.RegisterMessage(this, "applyNot", v => ((Array)v).ApplyNot());
         manager.RegisterMessage(this, "take", v => ((Array)v).Take());
         manager.RegisterMessage(this, "drop", v => ((Array)v).Drop());
         manager.RegisterMessage(this, "items", v => ((Array)v).GetItems());
         manager.RegisterMessage(this, "assign", v => ((Array)v).Assign());
         manager.RegisterMessage(this, "table", v => ((Array)v).Table(false));
         manager.RegisterMessage(this, "ltable", v => ((Array)v).Table(true));
         manager.RegisterMessage(this, "remove", v => ((Array)v).Remove());
         manager.RegisterMessage(this, "removeAll", v => ((Array)v).RemoveAll());
         manager.RegisterMessage(this, "while", v => ((Array)v).While());
         manager.RegisterMessage(this, "until", v => ((Array)v).Until());
         manager.RegisterMessage(this, "compact", v => ((Array)v).Compact());
         manager.RegisterMessage(this, "flat", v => ((Array)v).Flatten());
         manager.RegisterMessage(this, "sflat", v => ((Array)v).ShallowFlat());
         manager.RegisterMessage(this, "slice", v => ((Array)v).Slice());
         manager.RegisterMessage(this, "cons", v => ((Array)v).Cons());
         manager.RegisterMessage(this, "classify", v => ((Array)v).Classify());
         manager.RegisterMessage(this, "json", v => ((Array)v).JSON());
         manager.RegisterMessage(this, "concat", v => ((Array)v).Concat());
         manager.RegisterMessage(this, "range", v => ((Array)v).Range());
         manager.RegisterMessage(this, "assoc", v => ((Array)v).Assoc());
         manager.RegisterMessage(this, "trans", v => ((Array)v).Transpose());
         manager.RegisterMessage(this, "match", v => ((Array)v).Match());
         manager.RegisterMessage(this, "sum", v => ((Array)v).Sum());
         manager.RegisterMessage(this, "prod", v => ((Array)v).Product());
         manager.RegisterMessage(this, "avg", v => ((Array)v).Average());
         manager.RegisterMessage(this, "tail", v => ((Array)v).Tail());
         manager.RegisterMessage(this, "andify", v => ((Array)v).Andify());
         manager.RegisterMessage(this, "as", v => ((Array)v).As());
         manager.RegisterMessage(this, "fields", v => ((Array)v).Fields(false));
         manager.RegisterMessage(this, "awk", v => ((Array)v).Fields(true));
         manager.RegisterMessage(this, "groupNum", v => ((Array)v).GroupNumeric());
         manager.RegisterMessage(this, "break", v => ((Array)v).Break());
         manager.RegisterMessage(this, "on", v => ((Array)v).On());
         manager.RegisterMessage(this, "between", v => ((Array)v).Between(false));
         manager.RegisterMessage(this, "betweenx", v => ((Array)v).Between(true));
         manager.RegisterMessage(this, "and", v => ((Array)v).And());
         manager.RegisterMessage(this, "extend", v => ((Array)v).Extend());
         manager.RegisterMessage(this, "any", v => ((Array)v).SetAny());
         manager.RegisterMessage(this, "all", v => ((Array)v).SetAll());
         manager.RegisterMessage(this, "one", v => ((Array)v).SetOne());
         manager.RegisterMessage(this, "none", v => ((Array)v).SetNone());
         manager.RegisterMessage(this, "skip", v => ((Array)v).Skip());
         manager.RegisterMessage(this, "unfields", v => ((Array)v).Unfields());
         manager.RegisterMessage(this, "text", v => ((Array)v).AsText());
         manager.RegisterMessage(this, "when", v => ((Array)v).When());
         manager.RegisterMessage(this, "zipTo1", v => ((Array)v).ZipToOneArray());
         manager.RegisterMessage(this, "mapIf", v => ((Array)v).MapIf());
         manager.RegisterMessage(this, "seq", v => ((Array)v).Sequence());
         manager.RegisterMessage(this, "with", v => ((Array)v).With());
         manager.RegisterMessage(this, "text", v => ((Array)v).GetText());
         manager.RegisterMessage(this, "shiftUntil", v => ((Array)v).ShiftUntil());
         manager.RegisterMessage(this, "shiftWhile", v => ((Array)v).ShiftWhile());
         manager.RegisterMessage(this, "append", v => ((Array)v).Append());
         manager.RegisterMessage(this, "succ", v => ((Array)v).Succ());
         manager.RegisterMessage(this, "pred", v => ((Array)v).Pred());
         manager.RegisterMessage(this, "pair", v => ((Array)v).Pair());
         manager.RegisterMessage(this, "unpair", v => ((Array)v).Unpair());
         manager.RegisterMessage(this, "smap", v => ((Array)v).SelfMap());
         manager.RegisterMessage(this, "orderBy", v => ((Array)v).Order());
         //manager.RegisterMessage(this, "index", v => ((Array)v).Index());
         manager.RegisterMessage(this, "get", v => ((Array)v).Get());
         manager.RegisterProperty(this, "isAuto", v => ((Array)v).GetAutoAdd(), v => ((Array)v).SetAutoAdd());
         manager.RegisterProperty(this, "isMaybe", v => ((Array)v).GetMaybe(), v => ((Array)v).SetMaybe());
         manager.RegisterMessage(this, "alone", v => ((Array)v).Alone());
         manager.RegisterMessage(this, "fork", v => ((Array)v).Fork());
         manager.RegisterMessage(this, "head", v => ((Array)v).Head());
         manager.RegisterMessage(this, "shape", v => ((Array)v).Shape());
         manager.RegisterMessage(this, "span", v => ((Array)v).Span());
         manager.RegisterMessage(this, "enq", v => ((Array)v).Push());
         manager.RegisterMessage(this, "deq", v => ((Array)v).Shift());
         manager.RegisterMessage(this, "map2", v => ((Array)v).Map2());
         manager.RegisterMessage(this, "at", v => ((Array)v).GetItem());
         manager.RegisterMessage(this, "splitMap", v => ((Array)v).SplitMap());
         manager.RegisterMessage(this, "by", v => ((Array)v).By());
         manager.RegisterMessage(this, "cross", v => ((Array)v).Cross(false));
         manager.RegisterMessage(this, "rcross", v => ((Array)v).Cross(true));
         manager.RegisterMessage(this, "pzip", v => ((Array)v).PZip());
         manager.RegisterMessage(this, "in", v => ((Array)v).In());
         manager.RegisterMessage(this, "notIn", v => ((Array)v).NotIn());
         manager.RegisterMessage(this, "contains", v => ((Array)v).Contains());
         manager.RegisterMessage(this, "updateAt", v => ((Array)v).UpdateForKey());
         manager.RegisterMessage(this, "pairs", v => ((Array)v).Pairs());
         manager.RegisterMessage(this, "init", v => ((Array)v).Init());
         manager.RegisterMessage(this, "removeAt", v => ((Array)v).RemoveForKey());
         manager.RegisterMessage(this, "list", v => ((Array)v).List());
         manager.RegisterMessage(this, "indexed", v => ((Array)v).Indexed());
         manager.RegisterMessage(this, "gen", v => ((Array)v).Gen());
         manager.RegisterMessage(this, "insertSet", v => ((Array)v).InsertSet());
         manager.RegisterMessage(this, "swap", v => ((Array)v).Swap());
         manager.RegisterMessage(this, "array", v => ((Array)v).ToArray());
         manager.RegisterMessage(this, "alloc", v => ((Array)v).Alloc());
         manager.RegisterMessage(this, "exists", v => ((Array)v).Exists());
      }

      static IMaybe<int> getIntIndex(Value arg) => when(arg.Type == ValueType.Number, () => (int)arg.Number);

      static string getStrIndex(Value arg) => arg.Text;

      static IMaybe<int[]> getIntIndexes(Value[] values)
      {
         return when(values.All(v => v.Type == ValueType.Number), () => values.Select(v => (int)v.Number).ToArray());
      }

      static string[] getStrIndexes(Value[] values) => values.Select(v => v.Text).ToArray();

      public Value GetItem()
      {
         if (Length == 0)
            switch (array.Default)
            {
               case DefaultType.None:
                  return NullValue;
               case DefaultType.Value:
                  if (array.AutoAddDefault)
                     this[Arguments[0].Text] = array.DefaultValue;
                  return array.DefaultValue;
               case DefaultType.Lambda:
                  var key = Arguments[0].Text;
                  var value = array.DefaultLambda(key);
                  if (array.AutoAddDefault)
                     this[key] = value;
                  return value;
               default:
                  return new Array();
            }

         using (var popper = new RegionPopper(new Region(), "get-item"))
         {
            popper.Push();
            Regions.Current.SetParameter("$", Length);
            var iterator = new NSIteratorByLength(new Array(Arguments.GetValues(Length)).GetGenerator(), Length);
            var list = iterator.ToList();
            if (list.Count == 0)
               return new Array();

            var result = new Array(list.Select(getValue));
            return result.Length < 2 ? result[0] : result;
         }
      }

      Value getValue(Value value) => getIntIndex(value).FlatMap(index => this[index], () => this[getStrIndex(value)]);

      void setValue(Value value, Value valueToAssign)
      {
         if (getIntIndex(value).If(out var index))
            this[index] = valueToAssign;
         else
            this[getStrIndex(value)] = valueToAssign;
      }

      public Value SetItem()
      {
         var popped = Arguments.Values.Pop();
         if (popped.If(out var value))
         {
            var assignment = value.element.AssignmentValue();
            popped = popped.Value.array.Pop();
            if (popped.If(out value))
            {
               var index = value.element.AssignmentValue();

               Array indexArray;
               if (index.ProvidesGenerator && !assignment.ProvidesGenerator)
               {
                  indexArray = (Array)index.PossibleIndexGenerator().FlatMap(g => g.Array(), () => new Array { index });
                  if (assignment.IsNil)
                     for (var i = indexArray.Length - 1; i > -1; i--)
                        setValue(indexArray[i], assignment);
                  else
                     for (var i = 0; i < indexArray.Length; i++)
                        setValue(indexArray[i], assignment);

                  return this;
               }

               (var iArray, var assignArray) = CombineGenerators(index, assignment, false, false);
               indexArray = iArray;

               switch (indexArray.Length)
               {
                  case 2 when indexArray[0].Compare(indexArray[1]) == 0:
                     assignArray = (Array)assignArray.Flatten();
                     var indexAt = indexArray[0];
                     for (var i = assignArray.Length - 1; i > -1; i--)
                        insertAt(indexAt, assignArray[i]);

                     return this;
                  case 1:
                     setValue(indexArray[0], assignArray.Length == 1 ? assignArray[0] : assignArray);
                     return this;
               }

               if (assignment.IsNil)
                  for (var i = indexArray.Length - 1; i > -1; i--)
                     setValue(indexArray[i], assignment);
               else
                  for (var i = indexArray.Length - 1; i > -1; i--)
                     if (assignArray[i].IsNull)
                        Remove(i);
                     else
                        setValue(indexArray[i], assignArray[i]);

               return this;
            }

            return this;
         }

         return this;
      }

      public Value All()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  if (!block.IsTrue)
                     return false;
               }

               return true;
            }

            return Items.All(i => i.Value.IsTrue);
         }
      }

      public Value Any()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  if (block.IsTrue)
                     return true;
               }

               return false;
            }

            return Items.Any(i => i.Value.IsTrue);
         }
      }

      public Value None()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  if (block.IsTrue)
                     return false;
               }

               return true;
            }

            return Items.All(i => !i.Value.IsTrue);
         }
      }

      public Value Flatten()
      {
         var newArray = new Array();
         foreach (var item in this)
            if (item.Value.Type == ValueType.Array)
               foreach (var innerItem in (Array)((Array)item.Value).Flatten())
                  newArray.Add(innerItem.Value);
            else
               newArray.Add(item.Value);

         return newArray;
      }

      public Array ShallowFlat()
      {
         var newArray = new Array();
         foreach (var item in this)
            if (item.Value.IsArray)
               foreach (var innerItem in (Array)item.Value.SourceArray)
                  newArray.Add(innerItem.Value);
            else
               newArray.Add(item.Value);

         return newArray;
      }

      public virtual Value Remove()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var keysToRemove = new List<string>();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  if (block.Evaluate().IsTrue)
                     keysToRemove.Add(item.Key);
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
               }

               keysToRemove.Reverse();
               foreach (var keyToRemove in keysToRemove)
                  Remove(keyToRemove);

               return this;
            }

            var value = Arguments[0];
            var key = this.Where(i => value.Compare(i.Value) == 0).Select(i => i.Key).FirstOrDefault();
            if (key != null)
            {
               var element = this[key];
               Remove(key);
               return new Some(element);
            }

            return new None();
         }
      }

      public Value RemoveWithKey()
      {
         var value = Arguments[0];
         if (value.Type == ValueType.Number)
            Remove((int)value.Number);
         else
            Remove(value.Text);
         return this;
      }

      public Value Table(bool lines) => new Table(this, lines);

      public Value Assign()
      {
         foreach (var item in this)
         {
            var variableName = item.Key;
            if (variableName.IsNumeric())
               variableName = "$" + variableName;
            Regions[variableName] = item.Value.AssignmentValue();
         }

         return null;
      }

      public Value GetItems()
      {
         var result = new Array();
         for (var i = 0; i < array.Count; i++)
         {
            var key = GetKey(i);
            var value = this[i];
            var tuple = new OTuple(key, value);
            //tuple = tuple.Append(i);
            result.Add(tuple);
         }

         return result;
      }

      public Value Indexed() => new Array(Values.Select((v, i) => new Array { i, v }).ToArray());

      public Value Drop()
      {
         var count = (int)Arguments[0].Number;
         if (count > 0)
            return new Array(Values.RangeOf(count, Length - 1));
         if (count == 0)
            return this;

         var length = Length + count;
         return length < 0 || length > Length ? new Array() : new Array(Values.Slice(0, Length + count));
      }

      public Value Take()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               var newArray = new Array();
               assistant.ArrayParameters();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var result = block.Evaluate();
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

                  if (result.IsTrue)
                     newArray.Add(item.Value);
                  else
                     break;
               }

               return newArray;
            }

            var count = (int)Arguments[0].Number;
            if (count > 0)
            {
               if (count > Length)
                  count = Length;
               var newArray = new Array();
               for (var i = 0; i < count; i++)
                  newArray.Add(this[i]);

               return newArray;
            }

            if (count < 0)
            {
               count = -count;
               if (count > Length)
                  count = Length;
               var newArray = new Array();
               var offset = Length - count;
               for (var i = 0; i < count; i++)
               {
                  var index = i + offset;
                  newArray.Add(this[index]);
               }

               return newArray;
            }

            return this;
         }
      }

      public Value Apply()
      {
         var value = Arguments.ApplyValue;
         if (value.Type == ValueType.Symbol)
         {
            var messageName = value.Text;
            return new Message(messageName, new Arguments(this));
         }

         return ContainsValue(value);
      }

      public Value ApplyNot()
      {
         var value = Arguments.ApplyValue;
         return !ContainsValue(value);
      }

      public Value Rotate()
      {
         var count = (int)Arguments[0].Number;
         if (count == 0)
            count = 1;
         var newArray = CopyNoKeys();
         if (count > 0)
            for (var i = 0; i < count; i++)
            {
               var value = newArray.Shift();
               newArray.Add(value);
            }
         else
            for (var i = 0; i < -count; i++)
            {
               var value = newArray.Pop();
               newArray.Unshift(value);
            }

         return newArray;
      }

      public Value GetDefault()
      {
         switch (array.Default)
         {
            case DefaultType.Value:
               return array.DefaultValue;
            case DefaultType.Lambda:
               return array.DefaultLambda("");
            default:
               return new Nil();
         }
      }

      public Value SetDefault()
      {
         var defaultValue = Arguments[0].ArgumentValue();
         if (defaultValue.Type == ValueType.Lambda)
         {
            newValueLambda = (Lambda)defaultValue;
            array.Default = DefaultType.Lambda;
            array.DefaultLambda = key => newValueLambda.Evaluate(new Arguments(key));
         }
         else
         {
            array.Default = DefaultType.Value;
            array.DefaultValue = defaultValue;
         }
         return null;
      }

      public Value Default() => new ValueAttributeVariable("default", this);

      public Value Maybe() => new ValueAttributeVariable("isMaybe", this);

      public Value GetMaybe() => maybe;

      public Value SetMaybe()
      {
         maybe = Arguments[0].IsTrue;
         return null;
      }

      public Value Padder() => new Padder(this);

      static Array merge(Array source, Array other, Arguments arguments)
      {
         var newArray = new Array();
         foreach (var item in source)
            newArray[item.Key] = item.Value;

         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.MergeParameters();
               foreach (var key in other.Keys)
               {
                  var value2 = other[key];
                  if (newArray.ContainsKey(key))
                  {
                     var value1 = newArray[key];
                     assistant.SetMergeParameters(key, value1, value2);
                     var replacement = block.Evaluate();
                     newArray[key] = replacement;
                  }
                  else
                     newArray[key] = value2;
               }
            }
            else
            {
               foreach (var key in other.Keys)
                  newArray[key] = other[key];
            }

            return newArray;
         }
      }

      public virtual Value Merge()
      {
         var other = Arguments.AsArray();
         if (other == null)
         {
            if (Length <= 1)
               return this;

            var value = this[0];
            var source = value.IsArray ? (Array)value : new Array
            {
               value
            };
            for (var i = 1; i < Length; i++)
            {
               var value1 = this[i];
               var target = value1.IsArray ? (Array)value1 : new Array { value1 };
               source = merge(source, target, Arguments);
            }

            return source;
         }

         return merge(this, other, Arguments);
      }

      public Value First()
      {
         var countValue = Arguments[0];
         if (countValue.IsEmpty)
            return Length == 0 ? (Value)new None() : new Some(this[0]);

         var count = Math.Min((int)countValue.Number, Length);
         var newArray = new Array();
         for (var i = 0; i < count; i++)
            newArray.Add(this[i]);

         return new Some(newArray);
      }

      public Value Last()
      {
         var countValue = Arguments[0];
         if (countValue.IsEmpty)
            return Length == 0 ? (Value)new None() : new Some(this[-1]);

         var count = Math.Min((int)countValue.Number, Length);
         var newArray = new Array();
         var lastIndex = Length - count;
         for (var i = Length - 1; i >= lastIndex; i--)
            newArray.Add(this[i]);

         return new Some(newArray);
      }

      public Value RemoveWhere()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var keys = new List<string>();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var value = block.Evaluate();
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

                  if (value.IsTrue)
                     keys.Add(item.Key);
               }
               foreach (var key in keys)
                  Remove(key);

               return this;
            }

            return this;
         }
      }

      public Value Unfield() => array.ValueArray().Listify(State.FieldSeparator.Text);

      public Value Unrecord() => array.ValueArray().Listify(State.RecordSeparator.Text);

      public Value Splice()
      {
         var offset = (int)Arguments[0].Number;
         offset = WrapIndex(offset, Length, true);
         var length = (int)Arguments.DefaultTo(1, Length - offset).Number;
         length = WrapIndex(length, Length, true);

         var result = new Array();
         for (var i = offset + length - 1; i >= offset; i--)
         {
            result.Unshift(this[i]);
            Remove(i);
         }

         var listValue = Arguments[2];
         if (listValue.IsEmpty)
            return spliceResult(result);

         if (listValue.IsArray)
         {
            var list = (Array)listValue.SourceArray;
            foreach (var item in list.Reverse())
               Insert(offset, new KeyedValue(item.Key, item.Value));
         }
         else
            Insert(offset, listValue);

         return spliceResult(result);
      }

      static Value spliceResult(Array result)
      {
         switch (result.Length)
         {
            case 0:
               return "";
            case 1:
               return result[0];
            default:
               return result;
         }
      }

      public Value Invert()
      {
         var newArray = new Array();
         foreach (var item in this)
            if (item.Value.IsArray)
            {
               var valueArray = (Array)item.Value.SourceArray;
               invertArray(newArray, item.Key, valueArray);
            }
            else
            {
               var current = newArray[item.Value.Text];
               current = arrayify(current, item.Key);
               newArray[item.Value.Text] = current;
            }

         return newArray;
      }

      static void invertArray(Array newArray, string key, Array valueArray)
      {
         foreach (var item in valueArray)
            if (item.Value.IsArray)
            {
               var innerArray = (Array)item.Value.SourceArray;
               invertArray(newArray, key, innerArray);
            }
            else
            {
               var current = newArray[item.Value.Text];
               current = arrayify(current, key);
               newArray[item.Value.Text] = current;
            }
      }

      static Value arrayify(Value currentValue, Value newValue)
      {
         if (currentValue.IsEmpty)
            return newValue;

         var innerArray = currentValue.IsArray ? (Array)currentValue.SourceArray : new Array { currentValue };
         innerArray.Add(newValue);
         return innerArray;
      }

      public Value Sample()
      {
         var count = (int)Arguments[0].Number;
         if (count == 0)
         {
            var index = State.Random(Length);
            return this[index];
         }

         var newArray = new Array();
         var length = Length;
         for (var i = 0; i < count; i++)
            while (true)
            {
               var index = State.Random(length);
               var value = this[index];
               if (newArray.ContainsValue(value))
                  continue;

               newArray.Add(value);
               break;
            }

         return newArray;
      }

      public Value Shuffle()
      {
         var copy = Copy();
         var newArray = new Array();
         var random = new Random();
         var length = Length;
         while (length > 0)
         {
            var index = random.Next(length--);
            newArray.Add(copy[index]);
            copy.Remove(index);
         }

         return newArray;
      }

      public Value Any(Func<int, bool> func)
      {
         var count = 0;
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();

               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var value = block.Evaluate();
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

                  if (value.IsTrue)
                     count++;
               }

               return func(count);
            }

            return false;
         }
      }

      public bool MatchArrayAsList(Array comparisand, bool required)
      {
         var length = comparisand.Length;
         if (Length == 0)
            return length == 0;
         if (length == 0)
            return false;

         var bindings = new Hash<string, Value>();

         var last = length - 1;
         Value left;
         Value right;
         for (var i = 0; i < last; i++)
         {
            left = this[i];
            right = comparisand[i];
            if (right is Placeholder placeholder)
               bindings[placeholder.Text] = left;
            else if (!Case.Match(left, right, required, null))
               return false;
         }

         if (Length > length)
            left = FromIndexToEnd(last);
         else if (Length < length)
            left = new Array();
         else
            left = new Array { this[last] };
         right = comparisand[last];

         bool matched;
         if (right is Placeholder placeholder1)
         {
            bindings[placeholder1.Text] = left;
            matched = true;
         }
         else
            matched = Case.Match(left, right, required, null);
         if (matched)
            foreach (var item in bindings)
               Regions.SetParameter(item.Key, item.Value);

         return matched;
      }

      public bool MatchArray(Array comparisand, bool required, bool assigning)
      {
         var length = comparisand.Length;
         if (length == 0 && Length == 0)
            return true;
         if (Length != length)
            return false;

         for (var i = 0; i < length; i++)
         {
            var left = this[i];
            var right = comparisand[i];
            if (!Case.Match(left, right, required, null, assigning: assigning))
               return false;
         }

         return true;
      }

      public Array FromIndexToEnd(int index)
      {
         var newArray = new Array();
         for (var i = index; i < Length; i++)
            newArray.Add(this[i]);

         return newArray;
      }

      public Value Join()
      {
         if (Arguments.Executable.CanExecute)
         {
            var other = Arguments.AsArray();
            RejectNull(other, LOCATION, "First parameter of join must be an array");
            var var1 = Arguments.VariableName(0, VAR_X);
            var var2 = Arguments.VariableName(1, VAR_Y);
            var newArray = new Array();
            foreach (var item in this)
            foreach (var item2 in other)
            {
               Regions.SetLocal(var1, item.Value);
               Regions.SetLocal(var2, item2.Value);
               var value = Arguments.Executable.Evaluate();
               if (value != null)
                  newArray.Add(value);
            }

            return newArray;
         }

         var connector = Arguments[0].Text;
         return array.ValueArray().Select(v => v.Text).Listify(connector);
      }

      public Array Join(Array other)
      {
         var newArray = new Array();
         foreach (var outer in this)
         foreach (var inner in other)
         {
            var innerArray = new Array();
            flatJoin(innerArray, outer.Value);
            flatJoin(innerArray, inner.Value);
            newArray.Add(innerArray);
         }

         return newArray;
      }

      static void flatJoin(Array array, Value value)
      {
         if (value.IsArray)
            foreach (var item in (Array)value.SourceArray)
               flatJoin(array, item.Value);
         else
            array.Add(value);
      }

      public Value Count()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            int count;
            if (block == null)
            {
               var value = Arguments[0];
               if (value.IsEmpty)
                  return Length;

               count = this.Count(i => i.Value.Compare(value) == 0);
               return count;
            }

            assistant.ArrayParameters();
            count = 0;
            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var result = block.Evaluate();
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

               if (result.IsTrue)
                  count++;
            }

            return count;
         }
      }

      /*      public Value Find()
            {
               using (var assistant = new ParameterAssistant(Arguments))
               {
                  var block = assistant.Block();
                  if (block != null)
                  {
                     assistant.ArrayParameters();
                     foreach (var item in this)
                     {
                        assistant.SetParameterValues(item);
                        var result = block.Evaluate();
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

                        if (result.IsTrue)
                           return new Some(item.Value);
                     }

                     return new None();
                  }

                  var value = Arguments[0];
                  foreach (var item in this.Where(item => item.Value.Compare(value) == 0))
                     return new Some(item.Index);

                  return new None();
               }
            }*/

      public Value FindIndex()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var result = block.Evaluate();
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

                  if (result.IsTrue)
                     return new Some(item.Index);
               }

               return new None();
            }

            var value = Arguments[0];
            foreach (var item in this.Where(item => item.Value.Compare(value) == 0))
               return new Some(item.Index);

            return new None();
         }
      }

      public Value Max()
      {
         if (Length == 0)
            return new Nil();

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            Value max;
            if (block != null)
            {
               assistant.ArrayParameters();
               var items = Items;
               var first = items[0];
               assistant.SetParameterValues(first.Value, first.Key, 0);
               max = first.Value;
               var maxCompare = block.Evaluate();
               for (var i = 1; i < items.Length; i++)
               {
                  var item = items[i];
                  assistant.SetParameterValues(item.Value, item.Key, i);
                  var compare = block.Evaluate();
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

                  if (compare.Compare(maxCompare) <= 0)
                     continue;

                  maxCompare = compare;
                  max = item.Value;
               }

               return max;
            }

            max = this[0];
            for (var i = 1; i < Length; i++)
            {
               var value = this[i];
               if (value.Compare(max) > 0)
                  max = value;
            }

            return max;
         }
      }

      public Value Min()
      {
         if (Length == 0)
            return new Nil();

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            Value min;
            if (block != null)
            {
               assistant.ArrayParameters();
               var items = Items;
               var first = items[0];
               assistant.SetParameterValues(first.Value, first.Key, 0);
               min = first.Value;
               var minCompare = block.Evaluate();
               for (var i = 1; i < items.Length; i++)
               {
                  var item = items[i];
                  assistant.SetParameterValues(item.Value, item.Key, i);
                  var compare = block.Evaluate();
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

                  if (compare.Compare(minCompare) >= 0)
                     continue;

                  minCompare = compare;
                  min = item.Value;
               }

               return min;
            }

            min = this[0];
            for (var i = 1; i < Length; i++)
            {
               var value = this[i];
               if (value.Compare(min) < 0)
                  min = value;
            }

            return min;
         }
      }

      public Value Listify() => Values.Select(v => v.Text).Listify(Arguments[0].Text);

      public Value Fill()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            Value value;
            if (block != null)
            {
               assistant.ArrayParameters();
               foreach (var key in Keys)
               {
                  assistant.SetParameterValues(this[key], key, GetIndex(key));
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
                     this[key] = value;
               }

               return this;
            }

            value = Arguments[0];
            foreach (var key in Keys)
               this[key] = value.Clone();

            return this;
         }
      }

      public Value End() => Length - 1;

      public virtual Value Push()
      {
         var x = Arguments[0];
         Add(x);
         return this;
      }

      public Value Repeat(int count)
      {
         var newArray = new Array();
         for (var i = 0; i < count; i++)
            foreach (var item in this)
               newArray.Add(item.Value.AssignmentValue());

         return newArray;
      }

      public Value Repeat()
      {
         var x = Arguments[0];
         var count = (int)x.Number;
         return Repeat(count);
      }

      public Value ByKeys()
      {
         var source = Arguments[0];
         var result = new Array();
         if (source.Type == ValueType.Array)
         {
            var sourceArray = (Array)source;
            foreach (var item in sourceArray)
               sourceArray.Add(this[item.Value.Text]);
         }
         else
            result.Add(this[source.Text]);

         return result;
      }

      public Value ByIndexes()
      {
         var source = Arguments[0];
         var result = new Array();
         if (source.Type == ValueType.Array)
         {
            var sourceArray = (Array)source;
            foreach (var item in sourceArray)
               sourceArray.Add(this[(int)item.Value.Number]);
         }
         else
            result.Add(this[(int)source.Number]);

         return result;
      }

      Value ifValue(Value value) => value is Pattern pattern ? ifPattern(pattern) : ifText(value.Text);

      Value ifPattern(Pattern pattern)
      {
         var result = new Array();
         foreach (var item in this.Where(i => pattern.IsMatch(i.Value.Text)))
            result[item.Key] = item.Value;

         return result;
      }

      Value ifText(string text)
      {
         var result = new Array();
         foreach (var item in this.Where(i => i.Value.Text.Has(text)))
            result[item.Key] = item.Value;

         return result;
      }

      Value unlessValue(Value value) => value is Pattern pattern ? unlessPattern(pattern) : unlessText(value.Text);

      Value unlessPattern(Pattern pattern)
      {
         var result = new Array();
         foreach (var item in this.Where(i => !pattern.IsMatch(i.Value.Text)))
            result[item.Key] = item.Value;

         return result;
      }

      Value unlessText(string text)
      {
         var result = new Array();
         foreach (var item in this.Where(i => !i.Value.Text.Has(text)))
            result[item.Key] = item.Value;

         return result;
      }

      public Value While()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var result = new Array();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var value = block.Evaluate();
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

                  if (value.IsTrue)
                     result[item.Key] = item.Value;
                  else
                     break;
               }

               return result;
            }

            return null;
         }
      }

      public Value Until()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var result = new Array();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var value = block.Evaluate();
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

                  if (!value.IsTrue)
                     result[item.Key] = item.Value;
                  else
                     break;
               }

               return result;
            }

            return null;
         }
      }

      public Value SkipWhile() => skipWhile(Arguments);

      Value skipWhile(Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var result = new Array();
               var active = false;
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var value = block.Evaluate();
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

                  if (value.IsTrue && !active)
                     continue;

                  active = true;
                  result.Add(item.Value);
               }

               return result;
            }

            return null;
         }
      }

      public Value SkipUntil() => skipUntil(Arguments);

      Value skipUntil(Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var result = new Array();
               var active = false;
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var value = block.Evaluate();
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

                  if (!value.IsTrue && !active)
                     continue;

                  active = true;
                  result.Add(item.Value);
               }

               return result;
            }

            return null;
         }
      }

      public Value NotTop() => new Array(this.Where(i => i.Index > 0).Select(i => i.Value));

      public Value NotBottom() => new Array(this.Where(i => i.Index < Length - 1).Select(i => i.Value));

      public Value Middle() => new Array(this.Where(i => i.Index != 0 && i.Index != Length - 1).Select(i => i.Value));

      public Value NotMiddle() => new Array(this.Where(i => i.Index == 0 || i.Index == Length - 1).Select(i => i.Value));

      public virtual Value Insert()
      {
         var value = Arguments[0];
         var key = Arguments[1];

         return insertAt(key, value);
      }

      Value insertAt(Value key, Value value)
      {
         if (key.IsNumeric())
         {
            var index = (int)key.Number;
            switch (value.Type)
            {
               case ValueType.Array:
                  foreach (var item in (Array)value)
                     Insert(index, item.Value);

                  break;
               default:
                  Insert(index, value);
                  break;
            }

            return this;
         }

         var keyText = key.Text;
         switch (value.Type)
         {
            case ValueType.Array:
               foreach (var item in (Array)value)
                  Insert(keyText, item.Value);

               break;
            default:
               Insert(keyText, value);
               break;
         }

         return this;
      }

      public virtual Value AddUnique()
      {
         var value = Arguments[0];
         AddUnique(value);
         return this;
      }

      public virtual Value Unshift()
      {
         var value = Arguments[0];
         Unshift(value);
         return this;
      }

      public Value ContainsValue()
      {
         var value = Arguments[0];
         return ContainsValue(value);
      }

      public Value ContainsIndex() => ContainsIndex((int)Arguments[0].Number);

      public Value ContainsKey() => ContainsKey(Arguments[0].Text);

      public Value Delete()
      {
         var key = Arguments[0];
         Remove(key.Text);
         return null;
      }

      public virtual IEnumerator<IterItem> GetEnumerator() => new ArrayEnumerator(this);

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      /*      public Array Unique()
            {
               var unique = new Array();
               foreach (var value in Values)
                  unique.AddUnique(value);
               return unique;
            }*/

      public Value Zip()
      {
         var other = Arguments.AsArray();
         if (other == null)
            return this;

         return zip(other);
      }

      public Value ZipDo()
      {
         var other = Arguments.AsArray();
         if (other == null)
            return this;

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var newArray = new Array();
            var block = assistant.Block();
            assistant.TwoValueParameters();
            var length = options[OptionType.Max] ? Math.Max(Length, other.Length) : Math.Min(Length, other.Length);
            for (var i = 0; i < length; i++)
            {
               assistant.SetParameterValues(this[i], other[i]);
               var value = block.Evaluate();
               if (value != null)
                  newArray.Add(value);
            }

            return newArray;
         }
      }

      Value zip(Array other)
      {
         var newArray = new Array();
         for (var i = 0; i < Math.Min(Length, other.Length); i++)
            newArray.Add(OTuple.Concatenate(this[i], other[i]));

         using (var popper = new RegionPopper(new Region(), "zip"))
         {
            popper.Push();
            using (var assistant = new ParameterAssistant(Arguments))
            {
               var block = assistant.Block();
               if (block == null)
                  return newArray;

               var parameters = Arguments.Parameters;
               parameters.Splatting = true;
               var resultArray = new Array();
               foreach (var item in newArray)
               {
                  parameters.SetValues(item.Value);
                  var result = block.Evaluate();
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

                  resultArray.Add(result);
               }

               return resultArray;
            }
         }
      }

      Value flatZip(Array other)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var newArray = new Array();
            var block = assistant.Block();
            int length;
            if (block == null)
            {
               length = options[OptionType.Max] ? Math.Max(Length, other.Length) : Math.Min(Length, other.Length);
               for (var i = 0; i < length; i++)
               {
                  newArray.Add(this[i]);
                  newArray.Add(other[i]);
               }

               return newArray;
            }

            assistant.TwoValueParameters();
            length = options[OptionType.Max] ? Math.Max(Length, other.Length) : Math.Min(Length, other.Length);
            for (var i = 0; i < length; i++)
            {
               assistant.SetParameterValues(this[i], other[i]);
               var value = block.Evaluate();
               if (value != null)
                  newArray.Add(value);
            }

            return newArray;
         }
      }

      public Value ZipToOneArray()
      {
         var other = Arguments.AsArray();
         if (other == null)
            return this;

         var length = Math.Min(Length, other.Length);
         var newArray = new Array();
         for (var i = 0; i < length; i++)
            newArray[this[i].Text] = other[i];

         return newArray;
      }

      public Value Each()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var changes = new ArrayBase();

            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var result = block.Evaluate();
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

               if (result != null)
                  changes[item.Key] = result.AssignmentValue();
            }

            foreach (var item in changes)
               array[item.Key] = item.Value;

            return this;
         }
      }

      public Value Map()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var newArray = new Array();

            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return newArray;
               }

               if (value.IsNil)
                  continue;

               if (value is KeyedValue keyedValue)
                  newArray[keyedValue.Key] = keyedValue.Value;
               else
                  newArray[item.Key] = value;
            }

            return newArray;
         }
      }

      public Value Append()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var newArray = (Array)Clone();

            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return newArray;
               }

               if (value.Type != ValueType.Nil)
                  newArray.Add(value);
            }

            if (newArray.Length == 0)
               return new Nil();

            return newArray;
         }
      }

      public Value FlatMap()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var newArray = new Array();

            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var value = block.Evaluate();
               if (value.Type == ValueType.Nil)
                  continue;

               if (value is KeyedValue keyedValue)
                  value = keyedValue.Value;
               if (value.IsArray)
               {
                  var innerArray = (Array)value.SourceArray;
                  foreach (var innerItem in innerArray)
                     newArray.Add(innerItem.Value);
               }
               else
                  newArray.Add(item.Value);
            }

            if (newArray.Length == 0)
               return new Nil();

            return newArray;
         }
      }

      public int Compare(double x, double y) => x > y ? 1 : (x < y ? -1 : 0);

      public int Compare(string x, string y) => string.CompareOrdinal(x, y);

      public int Compare(Value x, Value y) => x.Compare(y);

      public override string ToString()
      {
         return "(" + Keys.Select(key => IsAutoAssignedKey(key) ? array[key].ToString() : $"{key.Quotify("`\"")} => {array[key]}")
            .Listify() + ")";
      }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index)
      {
         if (Length == 0)
            return NilValue;

         return index.Between(0).Until(Length) ? this[index] : NilValue;
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => this;

      public Value JSON()
      {
         return "[" + Keys.Select(key => IsAutoAssignedKey(key) ? array[key].ToString() : $"{key.Quotify("`\"")} : {array[key]}")
            .Listify() + "]";
      }

      static Value evaluateSortBlock(KeyValueBase item, ParameterAssistant assistant, Block block)
      {
         assistant.ArrayParameters();
         assistant.SetParameterValues(item.Value, item.Key, 0);
         return block.Evaluate();
      }

      Array sort(bool ascending)
      {
         var parameters = Arguments.Parameters;
         var length = parameters?.Length ?? 0;
         if (length < 1)
            return this;
         if (length == 1)
            return Sort(ascending);

         return Order();
      }

      public Array Sort(bool ascending)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            IOrderedEnumerable<KeyValuePair<string, Value>> sortedArray;
            if (block != null)
            {
               assistant.ArrayParameters();
               sortedArray = ascending ? array.OrderBy(i => evaluateSortBlock(i, assistant, block).Text, this)
                  : array.OrderByDescending(i => evaluateSortBlock(i, assistant, block).Text, this);
            }
            else
               sortedArray = ascending ? array.OrderBy(i => i.Value.Text, this) : array.OrderByDescending(i => i.Value.Text, this);
            return new Array(sortedArray);
         }
      }

      public Array SortNumeric(bool ascending)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            IOrderedEnumerable<KeyValuePair<string, Value>> sortedArray;
            if (block != null)
            {
               assistant.ArrayParameters();
               sortedArray = ascending ? array.OrderBy(i => evaluateSortBlock(i, assistant, block).Number, this)
                  : array.OrderByDescending(i => evaluateSortBlock(i, assistant, block).Number, this);
            }
            else
               sortedArray = ascending ? array.OrderBy(i => i.Value.Number, this) : array.OrderByDescending(i => i.Value.Number, this);
            return new Array(sortedArray);
         }
      }

      public Array Order()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.TwoValueParameters();
            var items = Items;
            System.Array.Sort(items, (i1, i2) => evaluateOrderBlock(i1.Value, i2.Value, assistant, block));
            return new Array(items);
         }
      }

      static int evaluateOrderBlock(Value value1, Value value2, ParameterAssistant assistant, Block block)
      {
         assistant.SetParameterValues(value1, value2);
         return (int)block.Evaluate().Number;
      }

      public Value GroupNumeric()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            var hash = new AutoHash<double, List<Value>>
            {
               Default = DefaultType.Lambda,
               DefaultLambda = k => new List<Value>(),
               AutoAddDefault = true
            };
            if (block == null)
               foreach (var item in this)
               {
                  var value = item.Value;
                  if (!value.IsArray)
                     continue;

                  var inner = (Array)value.SourceArray;
                  var key = inner[0].Number;
                  value = inner[1];
                  hash[key].Add(value);
               }
            else
            {
               assistant.ArrayParameters();
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var key = block.Evaluate().Number;
                  hash[key].Add(item.Value);
               }
            }

            var newArray = new Array();
            foreach (var item in hash)
               newArray[item.Key.ToString()] = new Array(item.Value);

            return newArray;
         }
      }

      public Value ReduceL()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               if (Length < 2)
                  return this;

               assistant.TwoValueParameters();

               var values = Values;
               var value = values[0];

               var result = new Array { value };

               for (var i = 1; i < values.Length; i++)
               {
                  assistant.SetParameterValues(value, values[i]);
                  value = block.Evaluate();
                  RejectNull(value, LOCATION, "Reduction block must return a value");
                  result.Add(value);
               }

               return result;
            }

            return new Array();
         }
      }

      public Value ReduceR()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               if (Length < 2)
                  return this;

               assistant.TwoValueParameters();

               var values = Values;
               var value = values[values.Length - 1];

               var result = new Array { value };

               for (var i = values.Length - 2; i > -1; i--)
               {
                  assistant.SetParameterValues(values[i], value);
                  value = block.Evaluate();
                  RejectNull(value, LOCATION, "Reduction block must return a value");
                  result.Add(value);
               }

               return result;
            }

            return new Array();
         }
      }

      public void RemoveAllValues(Value value)
      {
         var keysToDelete = this.Where(i => value.Compare(i.Value) == 0).Select(i => i.Key).ToList();
         foreach (var key in keysToDelete)
            Remove(key);
      }

      public Value RemoveAll()
      {
         var value = Arguments[0];
         RemoveAllValues(value);
         return this;
      }

      public Value IndexOf()
      {
         var value = Arguments[0];
         foreach (var item in this.Where(i => i.Value.Compare(value) == 0))
            return item.Index;

         return -1;
      }

      public Value KeyOf()
      {
         var value = Arguments[0];
         foreach (var item in this.Where(i => i.Value.Compare(value) == 0))
            return item.Key;

         return "";
      }

      public override bool IsEmpty => Length == 0;

      public Value Send(Value value, string messageName, Arguments arguments, out bool handled)
      {
         if (!messageName.StartsWith("$"))
         {
            handled = false;
            return null;
         }

         var key = messageName.Substring(1);
         handled = true;
         var block = new Block { new Push(key) };
         return new ChooseIndexer(this, block);
      }

      public bool RespondsTo(string messageName) => messageName.StartsWith("$");

      public override bool IsArray => true;

      public override Value SourceArray => this;

      public override Value SliceArray => SliceRangeInside(0, Length - 1, Length);

      public Value Compact()
      {
         var newArray = new Array();
         foreach (var item in this.Where(item => !item.Value.IsEmpty))
            newArray.Add(item.Value.AssignmentValue());

         return newArray;
      }

      public Value Slice()
      {
         var size = (int)Arguments[0].Number;
         var newArray = new Array();
         var list = new List<Value>();
         for (var i = 0; i < Length; i += size)
         {
            for (var j = 0; j < size; j++)
               list.Add(this[i + j]);

            var innerArray = new Array(list.ToArray());
            newArray.Add(innerArray);
            list.Clear();
         }

         return newArray;
      }

      public Value Cons()
      {
         var size = (int)Arguments[0].Number;
         var newArray = new Array();
         var list = new List<Value>();
         for (var i = 0; i < Length - size + 1; i++)
         {
            for (var j = 0; j < size; j++)
               list.Add(this[i + j]);

            var innerArray = new Array(list.ToArray());
            newArray.Add(innerArray);
            list.Clear();
         }

         return newArray;
      }

      public Value Classify()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            var newArray = new Array();
            assistant.ArrayParameters();
            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var key = block.Evaluate().Text;
               if (key.IsEmpty())
                  continue;

               var value = newArray[key];
               if (value.IsEmpty)
                  newArray[key] = item.Value;
               else if (value.IsArray)
                  newArray[key] = new Array { value, item.Value };
               else
               {
                  var innerArray = (Array)value.SourceArray;
                  innerArray.Add(item.Value);
                  newArray[key] = innerArray;
               }
            }

            return newArray;
         }
      }

      public Value Concat()
      {
         var asArray = Arguments.AsArray();
         return asArray == null ? ConcatenateValue(this, Arguments[0]) : Concatenate(this, asArray);
      }

      public Value Range() => new NSIntRange(0, Length, false);

      public Value Assoc()
      {
         var value = Arguments[0];
         foreach (var assoc in this.Where(i => i.Value.IsArray).Select(i => (Array)i.Value.SourceArray)
            .Where(a => a.ContainsValue(value)))
            return assoc;

         return new Nil();
      }

      public Value Transpose()
      {
         var list = new List<Array>();
         var maxLength = 0;
         foreach (var value in this.Select(item => item.Value))
         {
            Array subArray;
            if (value.IsArray)
               subArray = (Array)value.SourceArray;
            else
               subArray = new Array { value };
            list.Add(subArray);
            if (subArray.Length > maxLength)
               maxLength = subArray.Length;
         }

         var newArray = new List<Array>();
         for (var i = 0; i < maxLength; i++)
         {
            var subArray = new Array();
            newArray.Add(subArray);
            for (var j = 0; j < list.Count; j++)
               subArray[j] = list[j][i];
         }

         return new Array(newArray.ToArray());
      }

      public Value Match()
      {
         var matchSource = Arguments[0];
         var newArray = new Array();
         if (matchSource.Type == ValueType.Pattern)
         {
            var pattern = (Pattern)matchSource;
            foreach (var item in this.Where(item => pattern.IsMatch(item.Value.Text)))
               newArray[item.Key] = item.Value;
         }
         else
         {
            var source = matchSource.Text;
            foreach (var item in this.Where(item => item.Value.Text.Has(source, true)))
               newArray[item.Key] = item.Value;
         }

         return newArray;
      }

      public Value Sum() => this.Sum(i => i.Value.Number);

      public Value Product() => this.Aggregate(1d, (c, i) => c * i.Value.Number);

      public Value Average() => this.Average(i => i.Value.Number);

      public Value Tail()
      {
         if (Length <= 1)
            return new Array();

         return new Array(this.Where((item, i) => i > 0).Select(i => i.Value));
      }

      public Value Andify() => Values.Select(v => v.Text).ToArray().Andify();

      public Value As()
      {
         var block = Arguments.Executable;
         if (!block.CanExecute)
            return this;

         var arrayVar = Arguments.VariableName(0);
         var valueVar = Arguments.VariableName(1);
         var keyVar = Arguments.VariableName(2);
         var indexVar = Arguments.VariableName(3);

         block.AutoRegister = false;
         State.RegisterBlock(block);
         setVariable(VAR_ARRAY, arrayVar, this);
         setVariable(VAR_VALUE, valueVar);
         setVariable(VAR_KEY, keyVar);
         setVariable(VAR_INDEX, indexVar);

         var value = block.Evaluate();
         State.UnregisterBlock();
         return value;
      }

      static void setVariable(string runtimeVariable, string variable, Value value = null)
      {
         if (!variable.IsNotEmpty())
            return;

         Regions.SetLocal(runtimeVariable, variable);
         if (value != null)
            Regions.SetLocal(variable, value);
      }

      public Value Fields(bool awkify)
      {
         var splitter = Arguments[0];
         if (splitter.IsEmpty)
            splitter = State.FieldPattern;
         return splitter is Pattern pattern ? fields(pattern, awkify) : fields(splitter.Text, awkify);
      }

      Value fields(Pattern pattern, bool awkify)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var newArray = new Array();
            var block = assistant.Block();
            if (block == null)
            {
               foreach (var item in this)
               {
                  var value = item.Value;
                  var fields = value.IsArray ? (Array)value.SourceArray : new Array(pattern.Split(value.Text));
                  if (awkify)
                     fields.Unshift(value);
                  newArray.Add(fields);
               }

               return newArray;
            }

            var parameters = Arguments.Parameters;
            if (parameters == null || parameters.Length == 0)
               return this;

            foreach (var item in this)
            {
               var value = item.Value;
               var fields = value.IsArray ? (Array)value.SourceArray : new Array(pattern.Split(value.Text));
               if (awkify)
                  fields.Unshift(value);
               SetMultipleParameters(parameters, fields);
               var result = block.Evaluate();
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

               newArray.Add(result);
            }

            return newArray;
         }
      }

      Value fields(string pattern, bool awkify)
      {
         pattern = pattern.Escape();
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var newArray = new Array();
            var block = assistant.Block();
            if (block == null)
            {
               foreach (var item in this)
               {
                  var fields = new Array(item.Value.Text.Split(pattern));
                  if (awkify)
                     fields.Unshift(item.Value);
                  newArray.Add(fields);
               }

               return newArray;
            }

            var parameters = Arguments.Parameters;
            if (parameters == null || parameters.Length == 0)
               return this;

            foreach (var item in this)
            {
               var fields = new Array(item.Value.Text.Split(pattern));
               if (awkify)
                  fields.Unshift(item.Value);
               SetMultipleParameters(parameters, fields);
               var result = block.Evaluate();
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

               newArray.Add(result);
            }

            return newArray;
         }
      }

      public Value Break()
      {
         Message1 = new Message("break", Arguments.Clone());
         return this;
      }

      public Value On()
      {
         RejectNull(Message1, LOCATION, ".break message not sent");
         Assert(Message1.MessageName == "break", LOCATION, "previous message not .break");
         using (var breakAssistant = new ParameterAssistant(Message1.MessageArguments))
         {
            var breakBlock = breakAssistant.Block();
            if (breakBlock == null)
               return this;

            using (var onAssistant = new ParameterAssistant(Arguments))
            {
               var onBlock = onAssistant.Block();
               if (onBlock == null)
                  return this;

               var hash = new AutoHash<string, List<Value>>
               {
                  Default = DefaultType.Lambda,
                  DefaultLambda = k => new List<Value>(),
                  AutoAddDefault = true
               };
               breakAssistant.ArrayParameters();
               foreach (var item in this)
               {
                  breakAssistant.SetParameterValues(item);
                  var key = breakBlock.Evaluate().Text;
                  hash[key].Add(item.Value);
               }

               onAssistant.BreakOnParameters();
               var index = 0;
               var newArray = new Array();
               foreach (var item in hash)
               {
                  var key = item.Key;
                  var subarray = new Array(item.Value.ToArray());
                  onAssistant.SetBreakOnParameters(subarray, key, index++);
                  var value = onBlock.Evaluate();
                  newArray.Add(value);
               }

               return newArray;
            }
         }
      }

      Value beginValue(Value value) => value is Pattern pattern ? betweenPattern(pattern) : betweenText(value.Text);

      Value betweenPattern(Pattern pattern)
      {
         betweening = -1;
         for (var i = 0; i < Length; i++)
         {
            var input = this[i].Text;
            if (pattern.IsMatch(input))
            {
               betweening = i;
               break;
            }
         }

         return this;
      }

      Value betweenText(string text)
      {
         betweening = -1;
         for (var i = 0; i < Length; i++)
         {
            var input = this[i].Text;
            if (input.Has(text))
            {
               betweening = i;
               break;
            }
         }

         return this;
      }

      public Value Between(bool betweenExclusive)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return beginValue(Arguments[0]);

            assistant.ArrayParameters();
            betweening = -1;
            exclusive = betweenExclusive;
            for (var i = 0; i < Length; i++)
            {
               assistant.SetParameterValues(this[i], GetKey(i), i);
               var value = block.Evaluate();
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

               if (value.IsTrue)
               {
                  betweening = i;
                  break;
               }
            }

            return this;
         }
      }

      Value andValue(Value value) => value is Pattern pattern ? andPattern(pattern) : andText(value.Text);

      Value andPattern(Pattern pattern)
      {
         var newArray = new Array();
         for (var i = exclusive ? betweening - 1 : betweening; i < Length; i++)
         {
            var value = this[i];
            var input = value.Text;
            if (pattern.IsMatch(input))
            {
               if (!exclusive)
                  newArray.Add(value);
               break;
            }

            newArray.Add(value);
         }

         betweening = -1;
         return newArray;
      }

      Value andText(string text)
      {
         var newArray = new Array();
         for (var i = exclusive ? betweening - 1 : betweening; i < Length; i++)
         {
            var value = this[i];
            var input = value.Text;
            if (input.Has(text))
            {
               if (!exclusive)
                  newArray.Add(value);
               break;
            }

            newArray.Add(value);
         }

         betweening = -1;
         return newArray;
      }

      public Value And()
      {
         if (betweening == -1)
            return new Array();

         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return andValue(Arguments[0]);

            assistant.ArrayParameters();
            var newArray = new Array();
            for (var i = exclusive ? betweening - 1 : betweening; i < Length; i++)
            {
               var value = this[i];
               assistant.SetParameterValues(value, GetKey(i), i);
               var evaluation = block.Evaluate();
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

               if (evaluation.IsTrue)
               {
                  if (!exclusive)
                     newArray.Add(value);
                  break;
               }

               newArray.Add(value);
            }

            betweening = -1;
            return newArray;
         }
      }

      public Value Extend()
      {
         var newArray = Copy();
         foreach (var block in Arguments.Blocks)
            newArray.Add(block.Evaluate());

         return newArray;
      }

      public CompareType Comparison { get; set; }

      public Value SetAny()
      {
         Comparison = CompareType.Any;
         return this;
      }

      public Value SetAll()
      {
         Comparison = CompareType.All;
         return this;
      }

      public Value SetOne()
      {
         Comparison = CompareType.One;
         return this;
      }

      public Value SetNone()
      {
         Comparison = CompareType.None;
         return this;
      }

      public Value Skip()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            Array newArray;
            if (block != null)
            {
               assistant.ArrayParameters();
               var index = -1;
               for (var i = 0; i < Length; i++)
               {
                  var key = GetKey(i);
                  assistant.SetParameterValues(this[i], key, i);
                  var result = block.Evaluate();
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

                  index = i;
                  if (!result.IsTrue)
                     break;
               }

               newArray = new Array();
               for (var i = index; i < Length; i++)
                  newArray.Add(this[i]);

               return newArray;
            }

            var count = (int)Arguments[0].Number;
            if (count == 0)
               return this;

            if (count > 0)
            {
               if (count > Length)
                  count = Length;
               newArray = new Array();
               for (var i = count; i < Length; i++)
                  newArray.Add(this[i]);

               return newArray;
            }

            count = -count;
            if (count > Length)
               count = Length;
            count = Length - count;
            newArray = new Array();
            for (var i = 0; i < count; i++)
               newArray.Add(this[i]);

            return newArray;
         }
      }

      public Value Unfields()
      {
         var value = Arguments[0];
         var connector = value.IsEmpty ? State.FieldSeparator.Text : value.Text;
         var newArray = new Array();
         foreach (var item in this)
            if (item.Value.IsArray)
            {
               var newItem = ((Array)item.Value.SourceArray).Values.Listify(connector);
               newArray.Add(newItem);
            }
            else
               newArray.Add(item.Value);

         return newArray;
      }

      public Value AsText() => Values.Select(v => v.Text).Listify("");

      public Value When()
      {
         foreach (var value in Values)
            switch (value.Type)
            {
               case ValueType.When:
               {
                  var result = ((When)value).Invoke();
                  if (result.Type != ValueType.Nil)
                     return result;
               }

                  break;
               case ValueType.Block:
                  return ((Block)value).Evaluate();
               default:
                  return value;
            }

         return NilValue;
      }

      public Value MapIf()
      {
         var value = Arguments[0];
         if (!(value is When when))
            return this;

         var otherwiseExists = when.Otherwise != null;
         var otherwise = otherwiseExists ? (Block)when.Otherwise : null;
         using (var mapAssistant = new ParameterAssistant(when.Result))
         {
            var mapBlock = mapAssistant.Block();
            if (mapBlock == null)
               return this;

            mapAssistant.ArrayParameters();
            using (var ifAssistant = new ParameterAssistant(when.Condition))
            {
               var ifBlock = ifAssistant.Block();
               if (ifBlock == null)
                  return this;

               ifAssistant.ArrayParameters();
               var newArray = new Array();
               foreach (var item in this)
               {
                  mapAssistant.SetParameterValues(item);
                  if (ifBlock.Evaluate().IsTrue)
                  {
                     mapAssistant.SetParameterValues(item);
                     var result = mapBlock.Evaluate();
                     var signal = Signal();
                     if (signal == Breaking)
                        return newArray;

                     switch (signal)
                     {
                        case Continuing:
                           continue;
                        case ReturningNull:
                           return null;
                     }

                     if (result.Type != ValueType.Nil)
                        newArray[item.Key] = result;
                  }
                  else if (otherwiseExists)
                  {
                     mapAssistant.SetParameterValues(item);
                     var result = otherwise.Evaluate();
                     var signal = Signal();
                     if (signal == Breaking)
                        return newArray;

                     switch (signal)
                     {
                        case Continuing:
                           continue;
                        case ReturningNull:
                           return null;
                     }

                     if (result.Type != ValueType.Nil)
                        newArray[item.Key] = result;
                  }
                  else
                     newArray[item.Key] = item.Value;
               }

               return newArray;
            }
         }
      }

      public Value Sequence() => new PseudoStream(this);

      public Value With()
      {
         var block = Arguments.Executable;
         if (block.CanExecute)
         {
            block.AutoRegister = false;
            State.RegisterBlock(block);
            foreach (var item in this.Where(item => !IsAutoAssignedKey(item.Key)))
               Regions.SetLocal(item.Key, item.Value);

            block.Evaluate();
            State.UnregisterBlock();
         }

         return this;
      }

      public Value GetText() => Values.Select(v => v.Text).Listify("");

      public virtual Value ShiftUntil() => shiftUntil(Arguments);

      Value shiftUntil(Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();
            var newArray = new Array();
            while (Length > 0)
            {
               var value = shiftOne();
               assistant.SetParameterValues(value, "", 0);
               var result = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (!result.IsTrue)
                  newArray.Add(value);
               else
                  break;
            }

            return newArray;
         }
      }

      public Value ShiftWhile() => shiftWhile(Arguments);

      Value shiftWhile(Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();
            var newArray = new Array();
            while (Length > 0)
            {
               var value = shiftOne();
               assistant.SetParameterValues(value, "", 0);
               var result = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (result.IsTrue)
                  newArray.Add(value);
               else
                  break;
            }

            return newArray;
         }
      }

      public Value TakeUntil() => takeUntil(Arguments);

      Value takeUntil(Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            var newArray = new Array();
            assistant.ArrayParameters();
            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var result = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (result.IsTrue)
                  return newArray;

               newArray.Add(item.Value);
            }

            return newArray;
         }
      }

      public Value TakeWhile() => takeWhile(Arguments);

      Value takeWhile(Arguments arguments)
      {
         using (var assistant = new ParameterAssistant(arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            var newArray = new Array();
            assistant.ArrayParameters();
            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var result = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return null;
               }

               if (!result.IsTrue)
                  return newArray;

               newArray[item.Key] = item.Value;
               newArray.Add(item.Value);
            }

            return newArray;
         }
      }

      public Value Succ()
      {
         var newArray = new Array();
         foreach (var item in this)
            newArray[item.Key] = SendMessage(item.Value, "succ");

         return newArray;
      }

      public Value Pred()
      {
         var newArray = new Array();
         foreach (var item in this)
            newArray[item.Key] = SendMessage(item.Value, "pred");

         return newArray;
      }

      public Value Pair()
      {
         var newArray = new Array();
         for (var i = 0; i < Length; i += 2)
         {
            var key = this[i].Text;
            var value = this[i + 1];
            newArray[key] = value;
         }

         return newArray;
      }

      public Value Unpair()
      {
         var newArray = new Array();
         foreach (var item in this)
         {
            newArray.Add(item.Key);
            newArray.Add(item.Value);
         }

         return newArray;
      }

      public virtual Value SelfMap()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var newArray = new Array();

            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return newArray;
               }

               if (value.Type == ValueType.Nil)
                  continue;

               if (value is KeyedValue keyedValue)
                  newArray[keyedValue.Key] = keyedValue.Value;
               else
                  newArray[item.Key] = value;
            }

            if (newArray.Length == 0)
               return this;

            foreach (var item in newArray)
               this[item.Key] = item.Value;

            return this;
         }
      }

      /*      public Value Index()
            {
               var value = Arguments[0];
               var range = value.As<NSIntRange>();
               if (range.IsSome)
               {
                  range.Value.Length = Length.Some();
                  range.Value.Inside = true;
                  var wrapping = range.Value.As<IWrapping>();
                  if (wrapping.IsSome)
                     wrapping.Value.SetLength(Length);
                  return new IndexIndexer(this, (Array)value.SourceArray);
               }
               var keys = Arguments.AsArray(Length);
               if (keys == null)
                  return new IndexIndexer(this, new Array { Arguments[0] });
               keys = (Array)keys.Flatten();
               return new IndexIndexer(this, keys);
            }*/

      public Value Get()
      {
         var indicator = Arguments[0].Text;
         return ContainsKey(indicator) ? this[indicator] : Arguments[1];
      }

      public Value AutoAdd() => new ValueAttributeVariable("auto?", this);

      public Value SetAutoAdd()
      {
         array.AutoAddDefault = Arguments[0].IsTrue;
         return this;
      }

      public Value GetAutoAdd() => array.AutoAddDefault;

      public Value Set() => new Set(this);

      public Value Alone()
      {
         switch (Length)
         {
            case 0:
               return "";
            case 1:
               return this[0];
            default:
               return this;
         }
      }

      public Value Fork()
      {
         var newArray = (Array)Map();
         var closure = Arguments.Lambda;
         Block block = null;
         Parameters parameters = null;
         if (closure != null)
         {
            block = closure.Block;
            parameters = closure.Parameters;
         }
         return merge(this, newArray, new Arguments(new Block(), block, parameters));
      }

      public Value Head()
      {
         if (Length == 0)
            return "";

         return this[0];
      }

      public Value Shape()
      {
         var newArray = new Array();
         var rows = (int)Arguments[0].Number;
         var cols = (int)Arguments[1].Number;
         var index = 0;
         for (var i = 0; i < rows; i++)
         {
            var subArray = new Array();
            for (var j = 0; j < cols; j++)
               subArray.Add(this[index++]);

            newArray.Add(subArray);
         }

         return newArray;
      }

      public Value Span()
      {
         var taken = new Array();
         var skipped = new Array();
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block != null)
            {
               assistant.ArrayParameters();
               var taking = true;
               foreach (var item in this)
               {
                  assistant.SetParameterValues(item);
                  var result = block.Evaluate();
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

                  if (!result.IsTrue)
                     taking = false;
                  if (taking)
                     taken[item.Key] = item.Value;
                  else
                     skipped[item.Key] = item.Value;
               }
            }
            else
            {
               var count = (int)Arguments[0].Number;
               if (count < 0)
                  count = WrapIndex(count, Length, true);
               for (var i = 0; i < count; i++)
               {
                  var key = GetKey(i);
                  taken[key] = this[i];
               }
               for (var i = count; i < Length; i++)
               {
                  var key = GetKey(i);
                  skipped[key] = this[i];
               }
            }

            return new Array { taken, skipped };
         }
      }

      public Value Map2()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            assistant.ArrayParameters();

            var newArray = new Array();

            foreach (var item in this)
            {
               assistant.SetParameterValues(item);
               var value = block.Evaluate();
               var signal = Signal();
               if (signal == Breaking)
                  return newArray;

               switch (signal)
               {
                  case Continuing:
                     continue;
                  case ReturningNull:
                     return newArray;
               }

               if (value.Type == ValueType.Nil)
                  continue;

               string key;
               Value newValue;
               var subArray = new Array();
               if (value is KeyedValue keyedValue)
               {
                  key = keyedValue.Key;
                  newValue = keyedValue.Value;
                  newArray[key] = newValue;
               }
               else
               {
                  key = item.Key;
                  newValue = value;
               }
               subArray.Add(item.Value);
               subArray.Add(newValue);
               newArray[key] = subArray;
            }

            return newArray;
         }
      }

      public Value At()
      {
         var indicators = Arguments.AsArray();
         if (indicators != null)
         {
            if (indicators.Values.All(v => v.Type == ValueType.Number))
               return new IndexIndexer(this, indicators);

            var block = new Block { new Push(indicators) };
            return new ChooseIndexer(this, block);
         }

         var indicator = Arguments[0];
         if (indicator.Type == ValueType.Number)
            return new IndexIndexer(this, new Array { indicator });

         var keyBlock = new Block { new Push(indicator) };
         return new ChooseIndexer(this, keyBlock);
      }

      public Value SplitMap()
      {
         var pattern = Arguments[0] as Pattern;
         if (pattern == null)
            return this;

         var newArray = new Array();
         foreach (var item in this)
            newArray[item.Key] = new Array(pattern.Split(item.Value.Text));

         return newArray;
      }

      public Value By()
      {
         var count = (int)Arguments[0].Number;
         if (count < 2)
            return this;

         var newArray = new Array();
         for (var i = 0; i < Length; i += count)
         {
            var subarray = new Array();
            for (var j = 0; j < count; j++)
               subarray.Add(this[i + j]);

            newArray.Add(subarray);
         }

         return newArray;
      }

      public Value Cross(bool rightAssociate)
      {
         var rightArray = Arguments.AsArray();
         if (rightArray == null)
            return this;

         var sourceArray = (Array)Zip();
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            RejectNull(block, LOCATION, "No block found for cross");
            var newArray = new Array();
            var function = rightAssociate ? func<Array, Value>(a => new NSGenerator(a).FoldR())
               : func<Array, Value>(a => new NSGenerator(a).FoldL());
            foreach (var item in sourceArray)
            {
               var value = item.Value;
               if (value.IsArray)
               {
                  var inner = (Array)value.SourceArray;
                  inner.Arguments = Arguments.Clone();
                  value = function(inner);
                  newArray.Add(value);
               }
            }

            return newArray;
         }
      }

      public Value PZip() => new ParallelZip(this);

      public override Block AsBlock => null;

      public Value In() => ContainsValue(Arguments[0]);

      public Value NotIn() => !ContainsValue(Arguments[0]);

      public Value Contains() => ContainsValue(Arguments[0]);

      public IEnumerable<IterItem> GetTail()
      {
         for (var i = 1; i < Length; i++)
         {
            var value = this[i];
            var key = GetKey(i);
            yield return new IterItem(new KeyValueBase(key, value), i - 1);
         }
      }

      public Value Fold()
      {
         Assert(Length > 1, LOCATION, "Must have at least two elements for folding");
         var source = Arguments[0].Text;
         Assert(source.Length > 0, LOCATION, "Can't use an empty string in folding");
         var cumulative = false;
         var returnBlock = false;
         while (source.IsMatch("^ ['bc']"))
            switch (source[0])
            {
               case 'c':
                  cumulative = true;
                  source = source.Substring(1);
                  break;
               case 'b':
                  returnBlock = true;
                  source = source.Substring(1);
                  break;
               default:
                  source = source.Substring(1);
                  break;
            }

         var verbType = TwoCharacterOperatorParser.Operator(source);
         RejectNull(verbType, LOCATION, $"Didn't understand '{source}'");
         var verb = (Verb)Activator.CreateInstance(verbType);
         var builder = new CodeBuilder();
         if (cumulative)
         {
            Value value;
            Array accumulator;
            Block block;
            if (verb.LeftToRight)
            {
               value = this[0];
               builder.Value(value);
               accumulator = new Array { value };
               for (var i = 1; i < Length; i++)
               {
                  builder.Comma();
                  value = this[i];
                  accumulator.Add(value);
                  builder.Value(accumulator[0]);
                  for (var j = 1; j < accumulator.Length; j++)
                  {
                     builder.Verb(verb);
                     builder.Value(accumulator[j]);
                  }
               }

               block = builder.Block;
               return returnBlock ? block : block.Evaluate();
            }

            value = this[Length - 1];
            builder.Value(value);
            accumulator = new Array { value };
            for (var i = Length - 2; i >= 0; i--)
            {
               builder.Comma();
               value = this[i];
               builder.Value(value);
               for (var j = accumulator.Length - 1; j >= 0; j--)
               {
                  builder.Verb(verb);
                  builder.Value(accumulator[j]);
               }

               accumulator.Add(value);
            }

            block = builder.Block;
            return returnBlock ? block : block.Evaluate();
         }

         builder.Value(this[0]);
         builder.Verb(verb);
         builder.Value(this[1]);
         for (var i = 2; i < Length; i++)
         {
            builder.Verb(verb);
            builder.Value(this[i]);
         }

         var returnedBlock = builder.Block;
         return returnBlock ? returnedBlock : returnedBlock.Evaluate();
      }

      public Value UpdateForKey()
      {
         var value = Arguments[0];
         var key = Arguments[1];
         Value oldValue;
         switch (key.Type)
         {
            case ValueType.Number:
               var index = (int)key.Number;
               if (ContainsIndex(index))
               {
                  oldValue = this[index];
                  this[index] = value;
                  return new Some(oldValue);
               }

               return new None();
            default:
               var text = key.Text;
               if (ContainsKey(text))
               {
                  oldValue = this[text];
                  this[text] = value;
                  return new Some(oldValue);
               }

               return new None();
         }
      }

      Value removeAt(int index)
      {
         if (ContainsIndex(index))
         {
            var element = this[index];
            Remove(index);
            return new Some(element);
         }

         return new None();
      }

      Value removeAt(string key)
      {
         if (ContainsKey(key))
         {
            var element = this[key];
            Remove(key);
            return new Some(element);
         }

         return new None();
      }

      public Value RemoveForKey()
      {
         var key = Arguments[0];
         switch (key.Type)
         {
            case ValueType.Array:
               var returning = new Array();
               foreach (var value in ((Array)key).Select(item => item.Value).Select(value =>
                  value.Type == ValueType.Number ? removeAt((int)value.Number) : removeAt(value.Text)))
                  returning.Add(value);

               return returning;
            case ValueType.Number:
               return removeAt((int)key.Number);
            default:
               return removeAt(key.Text);
         }
      }

      public Value Pairs()
      {
         var newArray = new Array();
         foreach (var subArray in Items.Select(item => new Array { item.Key, item.Value }))
            newArray.Add(subArray);

         return newArray;
      }

      public Value Init()
      {
         if (Length == 0)
            return new Array();

         return new Array(this.Where((item, i) => i < Length - 1).Select(i => i.Value));
      }

      public Value List() => ArrayToList(this);

      public Value Gen() => new NSGenerator(this);

      public override Value AlternateValue(string message) => new NSGenerator(this);

      public Value InsertSet() => insertAt(Arguments[0], Arguments[1]);

      public Value Swap()
      {
         if (Length < 2)
            return this;

         var i = Arguments[0].Int;
         var j = Arguments[1].Int;
         i = WrapIndex(i, Length, true);
         j = WrapIndex(j, Length, true);
         if (i.Between(0).Until(Length) && j.Between(0).Until(Length))
         {
            var temp = this[i];
            this[i] = this[j];
            this[j] = temp;
         }
         return this;
      }

      public Value Alloc()
      {
         var count = Arguments[0].Int;
         var newArray = new Array();
         var innerArray = new Array();
         for (var i = 0; i < Length; i++)
         {
            innerArray.Add(this[i]);
            if (innerArray.Length == count)
            {
               newArray.Add(innerArray);
               innerArray = new Array();
            }
         }

         newArray.Add(innerArray);
         return newArray;
      }

      public Value Exists()
      {
         var key = Arguments[0].Text;
         return ContainsKey(key);
      }
   }
}
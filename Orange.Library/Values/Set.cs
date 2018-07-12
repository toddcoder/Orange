using System;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Enumerables;
using static Orange.Library.Runtime;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class Set : Value, IEqualityComparer<Value>, INSGeneratorSource
   {
      const string LOCATION = "Set";

      HashSet<Value> hashSet;
      Lazy<Value[]> values;

      public Set(Array array)
      {
         hashSet = new HashSet<Value>(array.Values, this);
         setValues();
      }

      void setValues() => values = new Lazy<Value[]>(getValues);

      public Set(SortedSet<Value> sortedSet)
      {
         hashSet = new HashSet<Value>(sortedSet.Select(v => v.Clone()), this);
         setValues();
      }

      public Set(IEnumerable<Value> enumerable)
      {
         hashSet = new HashSet<Value>(enumerable, this);
         setValues();
      }

      public Set()
      {
         hashSet = new HashSet<Value>(new Value[0], this);
         setValues();
      }

      Value[] getValues()
      {
         var elements = new Value[hashSet.Count];
         hashSet.CopyTo(elements);
         return elements;
      }

      public override int Compare(Value value) => 0;

      public bool IsSubsetOf(Set other) => hashSet.IsSubsetOf(other.hashSet);

      static Set getOtherSet(Arguments arguments)
      {
         var value = arguments[0];
         if (value is Set otherSet)
            return otherSet;

         Throw(LOCATION, $"{value} isn't a set");
         return null;
      }

      public Set getOtherSet() => getOtherSet(Arguments);

      public Value IsSubsetOf() => IsSubsetOf(getOtherSet(Arguments));

      public bool IsProperSubsetOf(Set other) => hashSet.IsProperSubsetOf(other.hashSet);

      public Value IsProperSubsetOf() => IsProperSubsetOf(getOtherSet(Arguments));

      public bool IsSupersetOf(Set other) => hashSet.IsSupersetOf(other.hashSet);

      public Value IsSupersetOf() => IsSupersetOf(getOtherSet(Arguments));

      public bool IsProperSupersetOf(Set other) => hashSet.IsProperSupersetOf(other.hashSet);

      public Value IsProperSupersetOf() => IsProperSupersetOf(getOtherSet(Arguments));

      public override string Text { get; set; }

      public override double Number { get; set; }

      public override ValueType Type => ValueType.Set;

      public override bool IsTrue => false;

      public override Value Clone() => new Set(hashSet);

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterMessage(this, "append", v => ((Set)v).Append());
         manager.RegisterMessage(this, "add", v => ((Set)v).Union());
         manager.RegisterMessage(this, "sub", v => ((Set)v).Difference());
         manager.RegisterMessage(this, "mult", v => ((Set)v).Intersection());
         manager.RegisterMessage(this, "in", v => ((Set)v).In());
         manager.RegisterMessage(this, "notIn", v => ((Set)v).NotIn());
         manager.RegisterMessage(this, "isSubset", v => ((Set)v).IsSubsetOf());
         manager.RegisterMessage(this, "isPropSubset", v => ((Set)v).IsProperSubsetOf());
         manager.RegisterMessage(this, "isSuperset", v => ((Set)v).IsSupersetOf());
         manager.RegisterMessage(this, "isPropSuperset", v => ((Set)v).IsProperSupersetOf());
         manager.RegisterMessage(this, "head", v => ((Set)v).Head());
         manager.RegisterMessage(this, "tail", v => ((Set)v).Tail());
         manager.RegisterMessage(this, "concat", v => ((Set)v).Concatenate());
         manager.RegisterMessage(this, "min", v => ((Set)v).Min());
         manager.RegisterMessage(this, "max", v => ((Set)v).Max());
         manager.RegisterMessage(this, "remove", v => ((Set)v).Remove());
         manager.RegisterMessage(this, "array", v => ((Set)v).ToArray());
      }

      public Value Append()
      {
         var value = Arguments[0];
         if (!value.IsNil)
            hashSet.Add(value);
         return this;
      }

      public void Add(Value value)
      {
         if (value is Set other)
            foreach (var innerValue in other.hashSet)
               Add(innerValue);
         else
            hashSet.Add(value);
      }

      public Value Union() => new Set(hashSet.Union(getOtherSet().hashSet));

      public Value Difference() => new Set(hashSet.Except(getOtherSet().hashSet));

      public Value Intersection() => new Set(hashSet.Intersect(getOtherSet().hashSet));

      public override Value AlternateValue(string message) => new Array(hashSet);

      public override string ToString() => $"({hashSet.Listify(" ^^ ")})";

      public bool Equals(Value x, Value y) => x.Compare(y) == 0;

      public int GetHashCode(Value obj) => obj.GetHashCode();

      public Value In() => hashSet.Contains(Arguments[0]);

      public Value NotIn() => !In().IsTrue;

      public bool Contains(Value value) => hashSet.Contains(value);

      public Value Head()
      {
         if (hashSet.Count == 0)
            return new Null();

         var first = hashSet.First();
         var set = new Set();
         set.Add(first);
         return set;
      }

      public Value Tail() => new Set(hashSet.Where((v, i) => i != 0));

      public override bool IsEmpty => hashSet.Count == 0;

      public Value Concatenate()
      {
         var otherSet = getOtherSet();
         var newSet = new Set(hashSet);
         foreach (var otherValue in otherSet.hashSet)
            newSet.Add(otherValue);

         return newSet;
      }

      public Value Min() => hashSet.Min();

      public Value Max() => hashSet.Max();

      public Value Remove()
      {
         var item = Arguments[0];
         hashSet.Remove(item);
         return this;
      }

      public INSGenerator GetGenerator() => new NSGenerator(this);

      public Value Next(int index)
      {
         if (index < hashSet.Count)
            return values.Value[index];

         setValues();
         return NilValue;
      }

      public bool IsGeneratorAvailable => true;

      public Array ToArray() => GeneratorToArray(this);
   }
}
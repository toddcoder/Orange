using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Managers;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Orange.Library.Managers.RegionManager;
using static Orange.Library.ParameterAssistant;
using static Orange.Library.ParameterAssistant.SignalType;
using static Orange.Library.Values.Nil;
using static Standard.Types.Arrays.ArrayFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Orange.Library.Values
{
   public class List : Value, IEnumerable<Value>
   {
      public static List FromArray(Array array)
      {
         switch (array.Length)
         {
            case 0:
               return new List();
            case 1:
               return new List(array[0]);
         }

         var stack = new Stack<Value>();
         foreach (var value in array.Values)
            stack.Push(value);
         while (stack.Count > 1)
         {
            var right = stack.Pop();
            var left = stack.Pop();
            var list = Cons(left, right);
            stack.Push(list);
         }

         return (List)stack.Pop();
      }

      public static List FromValue(Value value)
      {
         return value.PossibleIndexGenerator().FlatMap(generator => FromArray((Array)generator.Array()), () => new List(value));
      }

      public static List Cons(Value x, Value y) => y is List list ? new List(x, list) : new List(x, y);

      public static List Empty => new List();

      Value head;
      bool isEmpty;
      List tail;

      public List(Value head, Value tail)
      {
         this.head = head;
         this.tail = tail.IsNil ? Empty : new List(tail);
         isEmpty = false;
      }

      public List(Value head)
      {
         this.head = head;
         tail = Empty;
         isEmpty = false;
      }

      public List(Value head, List tail)
      {
         this.head = head;
         this.tail = tail;
         isEmpty = false;
      }

      public List() => isEmpty = true;

      public List Add(Value value)
      {
         if (isEmpty)
            return new List(value);

         var result = FoldR(Cons, new List(value), this);
         return result is List l ? l : new List(result);
      }

      public Value Head => isEmpty ? new List() : head;

      public List Tail
      {
         get => isEmpty ? Empty : tail;
         set => tail = value;
      }

      public Value GetHead() => isEmpty ? (Value)new None() : new Some(head);

      public Value GetTail() => tail;

      public override int Compare(Value value) => value is List l ? compareLists(this, l) : -1;

      static int compareLists(List left, List right)
      {
         if (left.IsEmpty && right.IsEmpty)
            return 0;
         if (left.IsEmpty)
            return 1;
         if (right.IsEmpty)
            return -1;

         var result = left.Head.Compare(right.Head);
         if (result == 0)
            return compareLists(left.Tail, right.Tail);

         return result;
      }

      public override string Text
      {
         get => $"{getText(this, true)}";
         set { }
      }

      public override double Number { get; set; }

      public override bool IsEmpty => isEmpty;

      public override ValueType Type => ValueType.List;

      public override bool IsTrue => !isEmpty && head.IsTrue;

      public override Value Clone() => this;

      protected override void registerMessages(MessageManager manager)
      {
         manager.RegisterProperty(this, "item", v => ((List)v).GetItem());
         manager.RegisterMessage(this, "head", v => ((List)v).GetHead());
         manager.RegisterMessage(this, "tail", v => ((List)v).GetTail());
         manager.RegisterMessage(this, "len", v => ((List)v).Length());
         manager.RegisterMessage(this, "map", v => ((List)v).Map());
         manager.RegisterMessage(this, "if", v => ((List)v).If());
         manager.RegisterMessage(this, "foldr", v => ((List)v).FoldR());
         manager.RegisterMessage(this, "foldl", v => ((List)v).FoldL());
         manager.RegisterMessage(this, "concat", v => ((List)v).Concat());
         manager.RegisterMessage(this, "find", v => ((List)v).Find());
         manager.RegisterMessage(this, "zip", v => ((List)v).Zip());
         manager.RegisterMessage(this, "take", v => ((List)v).Take());
         manager.RegisterMessage(this, "takeWhile", v => ((List)v).TakeWhile());
         manager.RegisterMessage(this, "takeUntil", v => ((List)v).TakeUntil());
         manager.RegisterMessage(this, "skip", v => ((List)v).Skip());
         manager.RegisterMessage(this, "skipWhile", v => ((List)v).SkipWhile());
         manager.RegisterMessage(this, "skipUntil", v => ((List)v).SkipUntil());
         manager.RegisterMessage(this, "last", v => ((List)v).Last());
         manager.RegisterMessage(this, "init", v => ((List)v).Init());
         manager.RegisterMessage(this, "split", v => ((List)v).Split());
         manager.RegisterMessage(this, "join", v => ((List)v).Join());
         manager.RegisterMessage(this, "for", v => ((List)v).For());
         manager.RegisterMessage(this, "add", v => ((List)v).Add());
         manager.RegisterMessage(this, "seq", v => ((List)v).Sequence());
         manager.RegisterMessage(this, "sort", v => ((List)v).Sort());
         manager.RegisterMessage(this, "sum", v => ((List)v).Sum());
         manager.RegisterMessage(this, "prod", v => ((List)v).Product());
         manager.RegisterMessage(this, "avg", v => ((List)v).Average());
         manager.RegisterMessage(this, "max", v => ((List)v).Max());
         manager.RegisterMessage(this, "min", v => ((List)v).Min());
         manager.RegisterMessage(this, "in", v => ((List)v).In());
         manager.RegisterMessage(this, "notIn", v => ((List)v).NotIn());
         manager.RegisterMessage(this, "isEmpty", v => ((List)v).IsEmpty);
         manager.RegisterMessage(this, "rev", v => ((List)v).Reverse());
         manager.RegisterMessage(this, "array", v => ((List)v).Array());
      }

      public IEnumerator<Value> GetEnumerator()
      {
         for (var current = this; !current.IsEmpty; current = current.Tail)
            yield return current.Head;
      }

      IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      public override string ToString() => $"[{getText(this, true)}]";

      protected static string getText(List list, bool first) =>
         list.isEmpty ? "" : $"{(first ? "" : ", ")}{list.head}{getText(list.Tail, false)}";

      public bool Match(List comparisand)
      {
         if (isEmpty || comparisand.IsEmpty)
            return isEmpty == comparisand.IsEmpty;

         var bindings = new Hash<string, Value>();
         if (Match(this, comparisand, bindings))
         {
            foreach (var item in bindings)
               Regions.SetParameter(item.Key, item.Value);

            return true;
         }

         return false;
      }

      public static bool Match(List left, List right, Hash<string, Value> bindings)
      {
         if (right.IsEmpty)
            return true;

         var lHead = left.Head;
         var rHead = right.Head;

         if (rHead is Placeholder || rHead is Any || Case.Match(lHead, rHead, false, null))
         {
            if (rHead is Placeholder placeholder)
               bindings[placeholder.Text] = right.Tail.IsEmpty ? left : lHead;
            return Match(left.Tail, right.Tail, bindings);
         }

         return false;
      }

      public static Value FoldR(Func<Value, Value, Value> func, Value accum, List list)
      {
         if (list.IsEmpty)
            return accum;

         return func(list.Head, FoldR(func, accum, list.Tail));
      }

      public static Value FoldR(Func<Value, Value, Value> func, Value accum, List list, Func<Value, List, bool> predicate)
      {
         if (list.IsEmpty || predicate(list.Head, list.Tail))
            return accum;

         return func(list.Head, FoldR(func, accum, list.Tail, predicate));
      }

      public static Value FoldL(Func<Value, Value, Value> func, Value accum, List list)
      {
         if (list.IsEmpty)
            return accum;

         return func(FoldL(func, accum, list.Tail), list.Head);
      }

      public static Value Map(Func<Value, Value> mappingFunc, List list)
      {
         return FoldR((a, b) => Cons(mappingFunc(a), b), new List(), list);
      }

      public static Value Filter(Func<Value, bool> filterFunc, List list)
      {
         return FoldR((a, b) => filterFunc(a) ? Cons(a, b) : b, new List(), list);
      }

      public static Value Append(List list1, List list2)
      {
         if (list1.IsEmpty)
            return list2;

         return new List(list1.head, Append(list1.Tail, list2.Tail));
      }

      public Value Length() => FoldR((v, a) => a.Int + 1, 0, this);

      public Value Map()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push("list-map");
            assistant.IteratorParameter();
            var result = Map(v =>
            {
               assistant.SetIteratorParameter(v);
               return block.Evaluate();
            }, this);
            Regions.Pop("list-map");
            return result;
         }
      }

      public Value If()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push("list-if");
            assistant.IteratorParameter();
            var result = Filter(v =>
            {
               assistant.SetIteratorParameter(v);
               return block.Evaluate().IsTrue;
            }, this);
            Regions.Pop("list-if");
            return result;
         }
      }

      public Value FoldR()
      {
         var accum = Arguments[0];
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push("foldr-list");
            assistant.TwoValueParameters();
            var result = FoldR((v, a) =>
            {
               assistant.SetParameterValues(v, a);
               return block.Evaluate();
            }, accum, this);
            Regions.Pop("foldr-list");
            return result;
         }
      }

      public Value FoldL()
      {
         var accum = Arguments[0];
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push("foldl-list");
            assistant.TwoValueParameters();
            var result = FoldL((v, a) =>
            {
               assistant.SetParameterValues(v, a);
               return block.Evaluate();
            }, accum, this);
            Regions.Pop("foldl-list");
            return result;
         }
      }

      public static Value Concat(List left, List right) => FoldR(Cons, right, left);

      public Value Concat() => Arguments[0] is List other ? Concat(this, other) : this;

      public Value Find()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push("find-list");
            assistant.IteratorParameter();
            var result = find(this, assistant, block);
            Regions.Pop("find-list");
            return result;
         }
      }

      static Value find(List list, ParameterAssistant assistant, Block block)
      {
         if (list.IsEmpty)
            return new None();

         var head = list.Head;
         assistant.SetIteratorParameter(head);
         if (block.Evaluate().IsTrue)
            return new Some(head);

         return find(list.Tail, assistant, block);
      }

      public Value Zip()
      {
         if (Arguments[0] is List right)
            using (var assistant = new ParameterAssistant(Arguments))
            {
               var block = assistant.Block();
               Func<Value, Value, Value> func;
               if (block == null)
                  func = (x, y) => new Array(array(x, y));
               else
                  func = (x, y) =>
                  {
                     assistant.SetParameterValues(x, y);
                     return block.Evaluate();
                  };
               Regions.Push("zip-list");
               assistant.TwoValueParameters();
               var result = zip(this, right, func);
               Regions.Pop("zip-list");
               return result;
            }

         return this;
      }

      static List zip(List left, List right, Func<Value, Value, Value> func)
      {
         if (left.IsEmpty)
            return left;
         if (right.IsEmpty)
            return right;

         return Cons(func(left.Head, right.Head), zip(left.Tail, right.Tail, func));
      }

      public Value process(Func<ParameterAssistant, Block, Value> func, string regionName)
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push(regionName);
            assistant.IteratorParameter();
            var result = func(assistant, block);
            Regions.Pop(regionName);
            return result;
         }
      }

      public Value Take()
      {
         var count = Arguments[0].Int;
         return take(count);
      }

      public Value TakeWhile() => process((a, b) => take((x, xs) =>
      {
         a.SetIteratorParameter(x);
         return !b.Evaluate().IsTrue;
      }), "take-while-list");

      public Value TakeUntil() => process((a, b) => take((x, xs) =>
      {
         a.SetIteratorParameter(x);
         return b.Evaluate().IsTrue;
      }), "take-until-list");

      public Value SkipWhile() => process((a, b) => skip((x, xs) =>
      {
         a.SetIteratorParameter(x);
         return !b.Evaluate().IsTrue;
      }), "skip-while-list");

      public Value SkipUntil() => process((a, b) => skip((x, xs) =>
      {
         a.SetIteratorParameter(x);
         return b.Evaluate().IsTrue;
      }), "skip-until-list");

      Value take(Func<Value, List, bool> predicate) => FoldR(Cons, Empty, this, predicate);

      public Value Skip()
      {
         var count = Arguments[0].Int;
         return skip(count);
      }

      Value skip(Func<Value, List, bool> predicate) => FoldR((v, a) => ((List)a).Tail, this, this, predicate);

      Value skip(int count) => skip((x, xs) => count-- <= 0);

      Value take(int count) => take((x, xs) => count-- <= 0);

      public Value Split()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
            {
               var count = Arguments[0].Int;
               return new Array { take(count), skip(count) };
            }

            assistant.IteratorParameter();
            using (var popper = new RegionPopper(new Region(), "list-split"))
            {
               popper.Push();
               var values = Values();
               var leftList = Empty;
               var rightList = Empty;
               foreach (var value in values)
               {
                  assistant.SetIteratorParameter(value);
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
                     leftList = leftList.Add(value);
                  else
                     rightList = rightList.Add(value);
               }

               var list = Empty;
               list = list.Add(leftList);
               list = list.Add(rightList);
               return list;
            }
         }
      }

      public Value Last() => skip((x, xs) => xs.IsEmpty);

      public Value Init() => take((x, xs) => xs.IsEmpty);

      public Value Join()
      {
         var connector = Arguments[0].Text;
         return FoldR((v, a) => a.Text.IsEmpty() ? v.Text : $"{v.Text}{connector}{a.Text}", "", this);
      }

      public Value[] Values()
      {
         Value list = new Array();
         var result = FoldR((v, a) =>
         {
            var array = (Array)list;
            array.Add(v);
            return list;
         }, list, this);
         return ((Array)result).Values;
      }

      public Value[] ComparisonValues()
      {
         Value list = new Array();
         var result = FoldR((v, a) =>
         {
            var array = (Array)list;
            array.Add(v);
            return list;
         }, list, this);
         var newArray = (Array)result;
         if (newArray.Length == 0)
            newArray.Add(Empty);
         return newArray.Values;
      }

      public Value For()
      {
         using (var assistant = new ParameterAssistant(Arguments))
         {
            var block = assistant.Block();
            if (block == null)
               return this;

            Regions.Push("list-for");
            assistant.IteratorParameter();

            foreach (var value in this)
            {
               assistant.SetIteratorParameter(value);
               block.Evaluate();
            }

            Regions.Pop("list-for");
            return this;
         }
      }

      public Value Add() => Add(Arguments[0]);

      public Value Sequence() => new ListSequence(this);

      public static Value Sort(List list)
      {
         if (list.isEmpty)
            return list;

         var head = list.head;
         var less = Sort((List)Filter(v => v.Compare(head) < 0, list.Tail));
         var greater = Sort((List)Filter(v => v.Compare(head) >= 0, list.Tail));
         var result = Concat((List)less, new List(head));
         return Concat((List)result, (List)greater);
      }

      public Value Sort() => Sort(this);

      public Value Sum() => FoldR((v, a) => a.Number + v.Number, 0, this);

      public Value Product() => FoldR((v, a) => a.Number * v.Number, 1, this);

      public Value Average() => Sum().Number / Length().Number;

      public Value Max() => FoldR((v, a) =>
      {
         if (a.IsNil)
            return v;

         return v.Compare(a) > 0 ? v : a;
      }, NilValue, this);

      public Value Min() => FoldR((v, a) =>
      {
         if (a.IsNil)
            return v;

         return v.Compare(a) < 0 ? v : a;
      }, NilValue, this);

      public Value In()
      {
         var target = Arguments[0];
         return this.Any(value => target.Compare(value) == 0);
      }

      public Value NotIn() => !In().IsTrue;

      public IMaybe<Value> this[int index]
      {
         get
         {
            var i = 0;
            foreach (var value in this)
            {
               if (i == index)
                  return value.Some();

               i++;
            }

            return none<Value>();
         }
      }

      public List Reverse()
      {
         if (isEmpty)
            return this;

         return tail.Reverse().Add(head);
      }

      public Value Array() => new Array(Values());

      public Value GetItem()
      {
         var index = Arguments[0].Int;
         return this[index].FlatMap(v => v, () => Empty);
      }
   }
}
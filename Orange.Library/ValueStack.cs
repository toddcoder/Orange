using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Enumerables;
using Standard.Types.Exceptions;
using static Standard.Types.Booleans.Assertions;

namespace Orange.Library
{
   public class ValueStack
   {
      Stack<Value> stack;

      public ValueStack() => stack = new Stack<Value>();

      public bool IsEmpty => stack.Count == 0;

      void assertStackNotEmpty(string location) => Reject(IsEmpty, $"Bad syntax at {location}");

      void assertStackNotEmpty() => Reject(IsEmpty, "Bad syntax");

      public void Push(Value value) => stack.Push(value);

      public Value Pop(bool resolve, string location, bool stringify = true)
      {
         assertStackNotEmpty(location);
         var value = stack.Pop();
         if (value != null && resolve)
         {
            value = value.Resolve();
            if (stringify && value is IStringify stringified)
               value = stringified.String;
         }
         return value;
      }

      public Value Pop(bool resolve, bool stringify = true)
      {
         assertStackNotEmpty();
         var value = stack.Pop();
         if (value != null && resolve)
         {
            value = value.Resolve();
            if (stringify && stringify && value is IStringify stringified)
               value = stringified.String;
         }
         return value;
      }

      public TValue Pop<TValue>(bool resolve, string location)
         where TValue : Value
      {
         var value = Pop(resolve, location);
         if (value is TValue tvalue)
            return tvalue;

         throw $"Expecting type {typeof(TValue).Name}".Throws();
      }

      public Value Peek(bool resolve, string location)
      {
         assertStackNotEmpty(location);
         var value = stack.Peek();
         if (resolve)
         {
            value = value.Resolve();
            value = stringify(value);
         }
         return value;
      }

      static Value stringify(Value value) => value is IStringify s ? s.String : value;

      public override string ToString() => IsEmpty ? "" : stack.Select(v => v.ToString()).Listify(" ");

      public void Clear() => stack.Clear();

      public Value[] ToArray() => stack.ToArray();
   }
}
using System.Collections.Generic;
using static Orange.Library.Values.Nil;

namespace Orange.Library.Values
{
   public class NSStackGenerator : NSGenerator
   {
      protected Stack<Value> stack;

      public NSStackGenerator(INSGeneratorSource generatorSource)
         : base(generatorSource) => stack = new Stack<Value>();

      public override void Reset()
      {
         stack.Clear();

         var iterator = new NSIterator(new NSGenerator(generatorSource));
         foreach (var value in iterator)
            stack.Push(value);
      }

      public override Value Next() => stack.Count == 0 ? NilValue : stack.Pop();
   }
}
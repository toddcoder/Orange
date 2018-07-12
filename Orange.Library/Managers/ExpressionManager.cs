using System.Collections.Generic;

namespace Orange.Library.Managers
{
   public class ExpressionManager
   {
      public enum VerbPresidenceType
      {
         NotApplicable = -1,
         Push = 0,
         SendMessage = 1,
         Invoke = 1,
         Indexer = 2,
         PreIncrement = 2,
         PreDecrement = 2,
         KeyedValue = 3,
         Increment = 4,
         Decrement = 4,
         ChangeSign = 4,
         Not = 4,
         Power = 5,
         Multiply = 6,
         Divide = 6,
         Mod = 6,
         Repeat = 6,
         Add = 7,
         Subtract = 7,
         Concatenate = 7,
         ShiftLeft = 8,
         ShiftRight = 8,
         LessThan = 9,
         LessThanEqual = 9,
         GreaterThan = 9,
         GreaterThanEqual = 9,
         Equals = 10,
         NotEqual = 10,
         BitAnd = 11,
         BitXOr = 12,
         BitOr = 13,
         And = 14,
         Or = 15,
         StopIncrement = 17,
         PushGraph = 17,
         Format = 18,
         CreateAlternator = 18,
         Range = 18,
         CreateTuple = 19,
         CreateArray = 20,
         When = 20,
         Apply = 21,
         Setter = 22,
         Statement = 23/*,
         Setter = 23*/
      }

      public static ExpressionManager ExpressionState { get; set; }

      Stack<VerbStack> verbStacks;

      public ExpressionManager() => verbStacks = new Stack<VerbStack>();

      public void Begin() => verbStacks.Push(new VerbStack());

      public void End() => verbStacks.Pop();

      public VerbStack Current => verbStacks.Peek();
   }
}
using System.Collections.Generic;
using System.Linq;
using Orange.Library.Values;
using static System.Math;
using static Orange.Library.Managers.MessageManager;
using static Orange.Library.Runtime;
using Array = Orange.Library.Values.Array;

namespace Orange.Library.Verbs
{
   public abstract class TwoValueVerb : Verb
   {
      public override Value Evaluate()
      {
         var stack = State.Stack;
         var y = stack.Pop(true, Location);
         if (y is Block yBlock && yBlock.Expression)
            y = yBlock.Evaluate();
         var x = stack.Pop(true, Location);
         if (x is Block xBlock && xBlock.Expression)
            x = xBlock.Evaluate();
         var result = Exception(x, y);
         if (result != null)
            return result;
         if (!(x is Object) && (x.IsArray || y.IsArray) && UseArrayVersion)
            return evaluateArray(x, y);

         return evaluate(x, y);
      }

      public virtual Value Exception(Value x, Value y) => null;

      Value evaluate(Value x, Value y)
      {
         if (y.Type == Value.ValueType.Complex && x.Type != Value.ValueType.Complex)
            x = new Complex(x.Number);
         if (y.Type == Value.ValueType.Big && x.Type != Value.ValueType.Big)
            x = new Big(x.Number);
         if (x.Type == Value.ValueType.Case && y.Type != Value.ValueType.Case)
         {
            var _case = (Case)x;
            y = new Case(_case.Value, y, false, _case.Required, _case.Condition);
         }
         switch (x.Type)
         {
            case Value.ValueType.Date:
            case Value.ValueType.Rational:
            case Value.ValueType.Complex:
            case Value.ValueType.Big:
            case Value.ValueType.Case:
            case Value.ValueType.Object:
            case Value.ValueType.Set:
               return MessagingState.SendMessage(x, Message, new Arguments(y));
            default:
               return (x.IsArray || y.IsArray) && UseArrayVersion ? evaluateArray(x, y) : Evaluate(x, y);
         }
      }

      Value evaluateArray(Value x, Value y)
      {
         if (y is KeyedValue)
            return SendMessage(x, Message, y);

         Array yArray;
         var list = new List<Value>();

         if (x.IsArray)
         {
            var xArray = (Array)x.SourceArray;
            if (y.IsArray)
            {
               yArray = (Array)y.SourceArray;
               var minLength = Min(xArray.Length, yArray.Length);
               for (var i = 0; i < minLength; i++)
               {
                  var xValue = xArray[i];
                  var yValue = yArray[i];
                  list.Add(evaluate(xValue, yValue));
               }

               return new Array(list);
            }

            list.AddRange(xArray.Select(i => evaluate(i.Value, y)));
            return new Array(list);
         }

         if (y.IsArray)
         {
            yArray = (Array)y.SourceArray;
            list.AddRange(yArray.Select(i => evaluate(x, i.Value)));
            return new Array(list);
         }

         return evaluate(x, y);
      }

      public abstract Value Evaluate(Value x, Value y);

      public abstract string Location { get; }

      public abstract string Message { get; }

      public virtual bool UseArrayVersion => true;
   }
}
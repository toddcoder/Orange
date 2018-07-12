using System;
using Orange.Library.Values;
using Standard.Types.Maybe;

namespace Orange.Library
{
   public static class BoundValuesHelper
   {
      public static IMaybe<Tuple<string, Value>> Unbind(this Value value)
      {
         return value.As<BoundValue>().Map(bv => tuple(bv.Name, bv.InnerValue));
      }
   }
}
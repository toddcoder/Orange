using System;
using System.Linq;
using Orange.Library.Values;
using Standard.Types.Exceptions;
using Standard.Types.Numbers;
using Standard.Types.RegularExpressions;
using Array = Orange.Library.Values.Array;
using Boolean = Orange.Library.Values.Boolean;

namespace Orange.Library.Invocations
{
   public static class TypeConvert
   {
      public static object ToType(this Value value, string type, bool isArray)
      {
         if (isArray)
         {
            if (value.IsArray)
            {
               value = value.SourceArray;
               var array = (Array)value;
               return array.Values.Select(v => v.ToType(type, false)).ToArray();
            }

            var newArray = new Array(Runtime.State.FieldPattern.Split(value.Text));
            return ToType(newArray, type, true);
         }

         switch (type)
         {
            case "system.int32":
               return (int)value.Number;
            case "system.int64":
               return (long)value.Number;
            case "system.int16":
               return (short)value.Number;
            case "system.byte":
               return (byte)value.Number;
            case "system.double":
               return value.Number;
            case "system.single":
               return (float)value.Number;
            case "system.boolean":
               return value.IsTrue;
            case "system.string":
               return value.Text;
            default:
               throw $"Didn't understand type {type}".Throws();
         }
      }

      public static object[] GetArguments(this Value[] arguments, string[] types)
      {
         var matcher = new Matcher();
         return types.Select((t, i) => t.GetArgument(arguments[i], matcher)).ToArray();
      }

      public static object GetArgument(this string type, Value value, Matcher matcher = null)
      {
         if (matcher == null)
            matcher = new Matcher();
         var isArray = matcher.IsMatch(type, "/(.+) /('[]') $");
         string typeName;
         if (isArray)
            matcher.Extract(0, out typeName, out var _);
         else
            typeName = type;
         return value.ToType(typeName, isArray);
      }

      public static object[] GetArguments(this Value[] arguments) => arguments.Select(a => a.GetArgument()).ToArray();

      public static object GetArgument(this Value argument)
      {
         if (argument is KeyedValue keyedValue)
         {
            var type = keyedValue.Key;
            var value = keyedValue.Value;
            return value.ToType(type, value.IsArray);
         }

         switch (argument.Type)
         {
            case Value.ValueType.Number:
               var number = argument.Number;
               var integer = (int)number;
               return integer == number ? integer : number;
            case Value.ValueType.Date:
               return ((Date)argument).DateTime;
            case Value.ValueType.Boolean:
               return argument.IsTrue;
            default:
               return argument.Text;
         }
      }

      public static Value GetValue(this object obj)
      {
         if (obj == null)
            return "";
         if (obj.IsNumeric())
            return (double)Convert.ChangeType(obj, typeof(double));
         if (obj is DateTime)
            return new Date((DateTime)obj);
         if (obj is bool)
            return new Boolean((bool)obj);

         return obj.ToString();
      }
   }
}
using System;

namespace Orange.Library.Values
{
	public static class Helper
	{
		 public static T ExtractValue<T>(this Value[] array, int index, Func<Value, T> func, T defaultValue = default(T))
		 {
			 if (index < 0 || index > array.Length)
				 return defaultValue;
			 return func(array[index]);
		 }
	}
}
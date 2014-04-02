using System;
using System.Collections.Generic;

namespace MultiCommandConsole.Util
{
	internal static class EnumerableExtensions
	{
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || enumerable.IsEmpty();
		}

		public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
		{
			return !enumerable.GetEnumerator().MoveNext();
		}

		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var item in enumerable)
			{
				action(item);
			}
		}

        public static T SafeFromIndex<T>(this IList<T> items, int index)
        {
            return items.Count > index ? items[index] : default(T);
        }

        public static T SafeFromIndex<T>(this T[] items, int index)
        {
            return items.Length > index ? items[index] : default(T);
        }
	}
}
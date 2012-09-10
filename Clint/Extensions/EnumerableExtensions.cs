using System.Collections.Generic;
using System.Linq;

namespace Clint.Extensions
{
	public static class e
	{
		public static IEnumerable<T> _<T>(params T[] a)
		{
			return a;
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
		{
			return e == null || !e.GetEnumerator().MoveNext();
		}
	}
}
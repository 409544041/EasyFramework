using System.Collections.Generic;
using System.Linq;

namespace UniEasy
{
	public static class LinqExtensions
	{
		// These are more efficient than Count() in cases where the size of the collection is not known
		public static bool HasAtLeast<T> (this IEnumerable<T> enumerable, int amount)
		{
			return enumerable.Take (amount).Count () == amount;
		}

		public static bool IsEmpty<T> (this IEnumerable<T> enumerable)
		{
			return !enumerable.Any ();
		}
	}
}

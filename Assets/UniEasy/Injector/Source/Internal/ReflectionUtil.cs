using System.Collections.Generic;
using System.Collections;
using System;

namespace UniEasy.DI
{
	public static class ReflectionUtil
	{
		public static IList CreateGenericList (Type elementType, object[] contentsAsObj)
		{
			var genericType = typeof(List<>).MakeGenericType (elementType);

			var list = (IList)Activator.CreateInstance (genericType);

			foreach (var obj in contentsAsObj) {
				list.Add (obj);
			}

			return list;
		}
	}
}

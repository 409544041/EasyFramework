using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable]
	public class EasyList<T>
	{
		[SerializeField]
		protected List<T> collection;

		public List<T> ToList ()
		{
			return collection;
		}

		public EasyList ()
		{
		}

		public EasyList (List<T> list)
		{
			collection = list;
		}

		public EasyList (params T[] values)
		{
			collection = new List<T> ();
			collection.AddRange (values);
		}
	}
}

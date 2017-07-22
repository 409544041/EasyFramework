using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable]
	public class EasyStringCollection : EasyList<string>
	{
		public EasyStringCollection (List<string> list)
		{
			collection = list;
		}

		public EasyStringCollection (params string[] values)
		{
			collection = new List<string> ();
			collection.AddRange (values);
		}
	}
}

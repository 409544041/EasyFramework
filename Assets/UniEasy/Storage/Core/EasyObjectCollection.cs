using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable]
	public class EasyObjectCollection : EasyList<EasyObject>
	{
		public EasyObjectCollection (List<EasyObject> list)
		{
			collection = list;
		}

		public EasyObjectCollection (EasyObject[] values)
		{
			collection = new List<EasyObject> ();
			collection.AddRange (values);
		}
	}
}

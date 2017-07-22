using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniEasy.Console
{
	[Serializable]
	public class DebugMask : EasyList<DebugLayer>
	{
		public DebugMask ()
		{
			this.collection = new List<DebugLayer> ();
		}

		public DebugMask (params string[] layerNames)
		{
			this.collection = new List<DebugLayer> ();
			for (int i = 0; i < layerNames.Length; i++) {
				this.collection.Add (new DebugLayer (true, layerNames [i]));
			}
		}

		public DebugMask (List<DebugLayer> value)
		{
			this.collection = value;
		}

		public DebugMask AddMask (bool isEnable, string layerName)
		{
			this.collection.Add (new DebugLayer (isEnable, layerName));
			return this;
		}
	}
}

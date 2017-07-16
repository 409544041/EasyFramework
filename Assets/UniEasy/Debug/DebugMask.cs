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
			this.value = new List<DebugLayer> ();
		}

		public DebugMask (params string[] layerNames)
		{
			this.value = new List<DebugLayer> ();
			for (int i = 0; i < layerNames.Length; i++) {
				this.value.Add (new DebugLayer (true, layerNames [i]));
			}
		}

		public DebugMask (List<DebugLayer> value)
		{
			this.value = value;
		}

		public DebugMask AddMask (bool isEnable, string layerName)
		{
			this.value.Add (new DebugLayer (isEnable, layerName));
			return this;
		}
	}
}

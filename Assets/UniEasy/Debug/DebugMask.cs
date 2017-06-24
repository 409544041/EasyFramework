using System.Collections.Generic;
using System;

namespace UniEasy.Console
{
	[Serializable]
	public class DebugMask : EasyList<DebugLayer>
	{
		public bool isLogEnabled = true;

		public bool IsLogEnabled {
			get {
				return isLogEnabled;
			}
			set {
				isLogEnabled = value;
			}
		}

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

		public DebugMask (DebugLayer[] value)
		{
			this.value = new List<DebugLayer> ();
			this.value.AddRange (value);
		}
	}
}

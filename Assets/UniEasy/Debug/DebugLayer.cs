using System;

namespace UniEasy.Console
{
	[Serializable]
	public struct DebugLayer
	{
		public bool isEnable;
		public string layerName;

		public DebugLayer (bool isEnable, string layerName)
		{
			this.isEnable = isEnable;
			this.layerName = layerName;
		}
	}
}

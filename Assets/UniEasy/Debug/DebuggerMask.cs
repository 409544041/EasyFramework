namespace UniEasy
{
	public class DebuggerMask
	{
		private string[] layerNames;

		public DebuggerMask (params string[] layerNames)
		{
			this.layerNames = layerNames;
		}

		public string[] GetLayerNames ()
		{
			return this.layerNames;
		}

		public void SetLayerNames (params string[] layerNames)
		{
			this.layerNames = layerNames;
		}
	}
}

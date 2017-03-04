using UnityEngine;
using System.Collections.Generic;
using System;

namespace UniEasy.ECS
{
	[Serializable]
	public struct DebugLayer
	{
		public bool IsEnable;
		public string LayerName;

		public DebugLayer (bool IsEnable, string LayerName)
		{
			this.IsEnable = IsEnable;
			this.LayerName = LayerName;
		}
	}

	[Serializable]
	public class DebugMask : EasyList<DebugLayer>
	{
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

	public class DebugSystem : SystemBehaviour
	{
		private DebugMask debugMask;
		private EasyWriter debugWriter;
		private readonly string Path = Application.streamingAssetsPath + "/DebugSystem/DefaultConfig.json";

		public EasyWriter DebugWriter {
			get {
				if (debugWriter == null)
					debugWriter = new EasyWriter (Path);
				return debugWriter;
			}
		}

		public DebugMask DebugMask {
			get {
				if (debugMask == null) {
					if (DebugWriter.HasKey ("Default"))
						debugMask = DebugWriter.Get<DebugMask> ("Default");
					else
						debugMask = new DebugMask (new List<DebugLayer> ());
				}
				return debugMask;
			}
			set {
				debugMask = value;
			}
		}

		protected override void Awake ()
		{
			base.Awake ();
		}

		void Start ()
		{
			
		}

		public void Dispose ()
		{
			DebugWriter.Set ("Default", DebugMask);
		}
	}
}
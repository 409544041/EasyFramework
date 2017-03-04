using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

		public string Path {
			get {
				return Application.streamingAssetsPath + "/DebugSystem/DefaultConfig.json";
			}
		}

		protected override void Awake ()
		{
			base.Awake ();
			// For performance consideration : 
			// We can't auto add a new layer when debug happened in every platform,
			// But you can add a new layer by hand or run in editor wait log output.
			Debugger.BeforeCheckLayerInEditorEvent += (layerName) => {
				if (!DebugMask.ToList ().Select (mask => mask.LayerName).Contains (layerName)) {
					var tempMask = DebugMask.ToList ();
					tempMask.Add (new DebugLayer (true, layerName));
					DebugMask = new DebugMask (tempMask);
					Refresh ();
					Dispose ();
				}
			};
		}

		void Start ()
		{
		}

		public void Refresh ()
		{
			var masks = DebugMask.ToList ();
			var layers = new List<string> ();
			for (int i = 0; i < masks.Count; i++) {
				if (masks [i].IsEnable) {
					layers.Add (masks [i].LayerName);
				}
			}
			Debugger.SetLayerMask (layers.ToArray ());
		}

		public void Dispose ()
		{
			DebugWriter.Set ("Default", DebugMask);
		}
	}
}
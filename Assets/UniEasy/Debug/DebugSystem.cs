using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

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
		public bool isLogEnabled = true;

		public bool IsLogEnabled {
			get {
				return isLogEnabled;
			}
			set {
				isLogEnabled = value;
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

	public class DebugSystem : SystemBehaviour
	{
		private const string DefaultDebugName = "default";
		private bool showOnUGUI;
		private DebugMask debugMask;
		private EasyWriter debugWriter;

		public string Path {
			get {
				return Application.streamingAssetsPath + "/DebugSystem/DefaultConfig.json";
			}
		}

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
					if (DebugWriter.HasKey (DefaultDebugName))
						debugMask = DebugWriter.Get<DebugMask> (DefaultDebugName);
					else
						debugMask = new DebugMask (new List<DebugLayer> ());
				}
				return debugMask;
			}
			set {
				debugMask = value;
			}
		}

		public bool ShowOnUGUI {
			get {
				return showOnUGUI;
			}
			set {
				showOnUGUI = value;
			}
		}

		public override void Setup ()
		{
			base.Setup ();
			// For performance consideration : 
			// We can't auto add a new layer when debug happened in every platform,
			// But you can add a new layer by hand or run in editor wait log output.
			Debugger.BeforeCheckLayerInEditorEvent += (layerName) => {
				if (!DebugMask.ToList ().Select (mask => mask.LayerName).Contains (layerName)) {
					var tempMask = DebugMask.ToList ();
					tempMask.Add (new DebugLayer (true, layerName));
					DebugMask = new DebugMask (tempMask);
					Refresh ();
					Save ();
				}
			};
			if (DebugWriter.HasKey ("showOnUGUI")) {
				showOnUGUI = DebugWriter.Get<bool> ("showOnUGUI");
			}
			Refresh ();

			var group = GroupFactory.Create (typeof(DebugCanvas));

			group.OnAdd ().Subscribe (entity => {
				var debugCanvas = entity.GetComponent<DebugCanvas> ();

				debugCanvas.canvas = debugCanvas.gameObject.AddComponent<Canvas> ();
				debugCanvas.canvasScaler = debugCanvas.gameObject.AddComponent<CanvasScaler> ();
				debugCanvas.graphicRaycaster = debugCanvas.gameObject.AddComponent<GraphicRaycaster> ();
				debugCanvas.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			}).AddTo (this.Disposer);
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
			Debugger.IsLogEnabled = DebugMask.IsLogEnabled;
			Debugger.ShowOnUGUI = this.ShowOnUGUI;
		}

		public void Save ()
		{
			DebugWriter.Set (DefaultDebugName, DebugMask);
			DebugWriter.Set ("showOnUGUI", ShowOnUGUI);
		}
	}
}

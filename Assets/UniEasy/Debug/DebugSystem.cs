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
		private BoolReactiveProperty showOnUGUI = new BoolReactiveProperty ();
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
				return showOnUGUI.Value;
			}
			set {
				showOnUGUI.Value = value;
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
				showOnUGUI.Value = DebugWriter.Get<bool> ("showOnUGUI");
			}
			Refresh ();

			var group = GroupFactory.Create (typeof(DebugCanvas));
			var viewEntities = GroupFactory.Create (typeof(DebugView));

			group.OnAdd ().Subscribe (entity => {
				var debugCanvas = entity.GetComponent<DebugCanvas> ();

				debugCanvas.canvas = debugCanvas.gameObject.AddComponent<Canvas> ();
				debugCanvas.canvasScaler = debugCanvas.gameObject.AddComponent<CanvasScaler> ();
				debugCanvas.graphicRaycaster = debugCanvas.gameObject.AddComponent<GraphicRaycaster> ();
				debugCanvas.canvas.renderMode = RenderMode.ScreenSpaceOverlay;

				viewEntities.OnAdd ().Subscribe (viewEntity => {
					var debugView = viewEntity.GetComponent<DebugView> ();

					debugView.panel = UIUtility.Create<RectTransform> ("DebugPanel", debugCanvas.transform);
					debugView.panel.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

					debugView.outputArea = UIUtility.Create<ScrollRect> ("OutputArea", debugView.panel.transform);
					debugView.outputArea.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					debugView.outputArea.viewport = UIUtility.Create<RectTransform> ("Viewport", debugView.outputArea.transform);
					debugView.outputArea.viewport.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					debugView.outputArea.viewport.gameObject.AddComponent<Image> ().raycastTarget = false;
					debugView.outputArea.viewport.gameObject.AddComponent<Mask> ().showMaskGraphic = false;
					var scrollbar = UIUtility.Create<Image> ("ScrollbarVertical", debugView.outputArea.transform);
					scrollbar.color = new Color32 (0x20, 0x20, 0x20, 0xFF);
					debugView.outputArea.horizontal = false;
					debugView.outputArea.verticalScrollbar = scrollbar.gameObject.AddComponent<Scrollbar> ();
					debugView.scrollbar = debugView.outputArea.verticalScrollbar;
					debugView.scrollbar.direction = Scrollbar.Direction.BottomToTop;
					debugView.scrollbar.targetGraphic = scrollbar;
					debugView.outputArea.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
					debugView.outputArea.verticalScrollbarSpacing = -3;
					debugView.scrollbar.transform.ToRectTransform (new Vector2 (1, 0), Vector2.one, new Vector2 (20, 0), new Vector2 (-10, 0));
					var slidingArea = UIUtility.Create<RectTransform> ("SlidingArea", debugView.scrollbar.transform);
					slidingArea.ToRectTransform (Vector2.zero, Vector2.one, new Vector2 (-20, -20), Vector2.zero);
					var handle = UIUtility.Create<Image> ("Handle", slidingArea);
					handle.transform.ToRectTransform (Vector2.zero, Vector2.one, new Vector2 (20, 20), Vector2.zero);
					debugView.scrollbar.targetGraphic = handle;
					debugView.scrollbar.handleRect = handle.rectTransform;
					debugView.outputText = UIUtility.Create<Text> ("OutputText", debugView.outputArea.viewport.transform);
					debugView.outputText.gameObject.AddComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
					debugView.outputText.ToConfigure (new Color32 (0x32, 0x32, 0x32, 0xFF), raycastTarget: false);
					debugView.outputText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					debugView.outputArea.content = (RectTransform)debugView.outputText.transform;

					showOnUGUI.DistinctUntilChanged ().Subscribe (b => {
						debugView.panel.gameObject.SetActive (b);
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer);

					Debugger.OnLogEvent += (type, message) => {
						if (debugView != null && debugView.outputText != null) {
							if (type == LogType.Warning) {
								message = string.Format ("<b><size={0}><color=#ffff00ff>{1}</color></size></b>", debugView.size, message);
							} else if (type == LogType.Error) {
								message = string.Format ("<b><size={0}><color=#ff0000ff>{1}</color></size></b>", debugView.size, message);
							} else {
								message = string.Format ("<b><size={0}><color=#ffffffff>{1}</color></size></b>", debugView.size, message);
							}
							if (string.IsNullOrEmpty (debugView.outputText.text)) {
								debugView.outputText.text += message;
							} else {
								debugView.outputText.text += Environment.NewLine + message;
							}
						}
					};
				}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer);
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

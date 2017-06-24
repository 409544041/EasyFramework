using UniEasy.Console;
using UnityEngine.UI;
using UniRx.Triggers;
using UniEasy.ECS;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

namespace UniEasy.Console
{
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
						debugMask = new DebugMask ();
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
				if (!DebugMask.ToList ().Select (mask => mask.layerName).Contains (layerName)) {
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
				debugCanvas.canvas.sortingOrder = 100;

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
					debugView.outputArea.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
					debugView.outputArea.verticalScrollbarSpacing = -3;
					debugView.scrollbar.transform.ToRectTransform (new Vector2 (1, 0), Vector2.one, new Vector2 (20, 0), new Vector2 (-10, 0));
					var slidingArea = UIUtility.Create<RectTransform> ("SlidingArea", debugView.scrollbar.transform);
					slidingArea.ToRectTransform (Vector2.zero, Vector2.one, new Vector2 (-20, -20), Vector2.zero);
					var handle = UIUtility.Create<Image> ("Handle", slidingArea);
					handle.transform.ToRectTransform (Vector2.zero, Vector2.one, new Vector2 (20, 20), Vector2.zero);
					debugView.scrollbar.targetGraphic = handle;
					debugView.scrollbar.handleRect = handle.rectTransform;

					debugView.outputPanel = UIUtility.Create<RectTransform> ("OutputPanel", debugView.outputArea.viewport.transform);
					debugView.outputPanel.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					debugView.outputLayout = debugView.outputPanel.gameObject.AddComponent<VerticalLayoutGroup> ();
					debugView.outputLayout.childAlignment = TextAnchor.UpperLeft;
					debugView.outputLayout.childControlHeight = true;
					debugView.outputLayout.childControlWidth = true;
					debugView.outputLayout.childForceExpandHeight = false;
					debugView.outputLayout.childForceExpandWidth = false;
					debugView.outputPanel.gameObject.AddComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

//					debugView.outputText = UIUtility.Create<Text> ("OutputText", debugView.outputArea.viewport.transform);
//					debugView.outputText.gameObject.AddComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
//					debugView.outputText.ToConfigure (Color.white, raycastTarget: false);
//					debugView.outputText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
//					debugView.outputArea.content = (RectTransform)debugView.outputText.transform;
//
//					debugView.collapseCountText = UIUtility.Create<Text> ("CollapseCountText", debugView.outputText.transform);
//					debugView.collapseCountText.ToConfigure (Color.white, raycastTarget: false);
//					debugView.collapseCountText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
//					debugView.collapseCountText.alignment = TextAnchor.UpperRight;

					debugView.menuPanel = UIUtility.Create<RectTransform> ("MenuPanel", debugView.panel.transform);
					debugView.menuPanel.ToRectTransform (new Vector2 (0, 1), Vector2.one, new Vector2 (0, 20), new Vector2 (0, -10));

					var escImage = UIUtility.Create<Image> ("EscButton", debugView.menuPanel.transform);
					escImage.transform.ToRectTransform (Vector2.zero, new Vector2 (0, 1), new Vector2 (40, 0), new Vector2 (20, 0));
					debugView.escButton = escImage.gameObject.AddComponent<Button> ();
					var escText = UIUtility.Create<Text> ("EscText", debugView.escButton.transform);
					escText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					escText.ToConfigure (new Color32 (0x20, 0x20, 0x20, 0xFF), alignment : TextAnchor.MiddleCenter, fontSize : 12);
					escText.text = "Esc";

					debugView.escButton.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.panel.gameObject.SetActive (false);
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					showOnUGUI.DistinctUntilChanged ().Subscribe (b => {
						debugView.panel.gameObject.SetActive (b);
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer);

					CommandLibrary.RegisterCommand ("Debug", "Show debug console message on UGui.", "Debug [on/off]", (args) => {
						if (args.Length == 0) {
							return "Sorry, it is a invalid command!";
						} else {
							bool result = args [0].ToLower ().Equals ("on") ? true : false;
							debugView.panel.gameObject.SetActive (result);
							return string.Format ("Debug message show on UGui is {0}", result == true ? "On" : "Off");
						}
					});

					var clearImage = UIUtility.Create<Image> ("ClearButton", debugView.menuPanel.transform);
					clearImage.transform.ToRectTransform (Vector2.zero, new Vector2 (0, 1), new Vector2 (40, 0), new Vector2 (65, 0));
					debugView.clearButton = clearImage.gameObject.AddComponent<Button> ();
					var clearText = UIUtility.Create<Text> ("ClearText", debugView.clearButton.transform);
					clearText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					clearText.ToConfigure (new Color32 (0x20, 0x20, 0x20, 0xFF), alignment : TextAnchor.MiddleCenter, fontSize : 12);
					clearText.text = "Clear";

					debugView.clearButton.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.logs.Clear ();
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					var collapseImage = UIUtility.Create<Image> ("CollapseToggle", debugView.menuPanel.transform);
					collapseImage.transform.ToRectTransform (Vector2.zero, new Vector2 (0, 1), new Vector2 (60, 0), new Vector2 (120, 0));
					debugView.collapseToggle = collapseImage.gameObject.AddComponent<Toggle> ();
					var collapseCheckmark = UIUtility.Create<Image> ("CollapseCheckmark", debugView.collapseToggle.transform);
					collapseCheckmark.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					collapseCheckmark.color = new Color32 (0xCC, 0xCC, 0xCC, 0xFF);
					debugView.collapseToggle.graphic = collapseCheckmark;
					var collapseText = UIUtility.Create<Text> ("CollapseText", debugView.collapseToggle.transform);
					collapseText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
					collapseText.ToConfigure (new Color32 (0x20, 0x20, 0x20, 0xFF), alignment : TextAnchor.MiddleCenter, fontSize : 12);
					collapseText.text = "Collapse";

					debugView.collapseToggle.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.collapse = debugView.collapseToggle.isOn;
						debugView.Refresh = true;
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					Debugger.OnLogEvent += (type, message) => {
						if (debugView != null && debugView.outputPanel != null) {
							if (type == LogType.Warning) {
								message = string.Format ("<b><size={0}><color=#ffff00ff>{1}</color></size></b>", debugView.size, message);
							} else if (type == LogType.Error) {
								message = string.Format ("<b><size={0}><color=#ff0000ff>{1}</color></size></b>", debugView.size, message);
							} else {
								message = string.Format ("<b><size={0}><color=#ffffffff>{1}</color></size></b>", debugView.size, message);
							}
							debugView.lines.Add (new DebugLine (type, debugView.outputPanel));
							debugView.logs.Add (message.ToString ());
						}
					};

					debugView.logs.ObserveAdd ().Subscribe (x => {
						if (debugView.collapseLogs.ContainsKey (x.Value)) {
							debugView.collapseLogs [x.Value]++;
						} else {
							debugView.collapseLogs.Add (x.Value, 1);
						}
						debugView.Refresh = true;
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.logs.ObserveReset ().Subscribe (_ => {
//						debugView.outputText.text = "";
						debugView.collapseLogs.Clear ();
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.refresh.DistinctUntilChanged ().Where (b => b == true).Subscribe (_ => {
//						var lineCount = debugView.collapse == true ? debugView.collapseLogs.Count : debugView.logs.Count;
//						var anchor = 1f / lineCount;
//						for (int i = 0; i < lineCount; i++) {
//							Image bg;
//							if (i < debugView.backgrouds.Count) {
//								bg = debugView.backgrouds [i];
//							} else {
//								bg = UIUtility.Create<Image> ("OutputBackgroud", debugView.outputText.transform);
//								debugView.backgrouds.Add (bg);
//							}
//							bg.transform.ToRectTransform (new Vector2 (0, 1 - anchor * (i + 1)), new Vector2 (1, 1 - anchor * i), Vector2.zero, Vector2.zero);
//							if (i % 2 == 0) {
//								bg.color = new Color32 (0x00, 0x00, 0x00, 0x48);
//								bg.gameObject.SetActive (true);
//							} else {
//								bg.gameObject.SetActive (false);
//							}
//						}
//						if (debugView.collapse) {
//							var logs = debugView.collapseLogs.Keys.ToArray ();
//							var counts = debugView.collapseLogs.Values.ToArray ();
//							for (int i = 0; i < logs.Length; i++) {
//								if (i == 0) {
//									debugView.outputText.text = logs [i];
//									debugView.collapseCountText.text = string.Format ("({0})", counts [i]);
//								} else {
//									debugView.outputText.text += Environment.NewLine + logs [i];
//									debugView.collapseCountText.text += Environment.NewLine + string.Format ("({0})", counts [i]);
//								}
//							}
//							debugView.collapseCountText.rectTransform.SetAsLastSibling ();
//						} else {
//							debugView.collapseCountText.text = "";
//							for (int i = 0; i < debugView.logs.Count; i++) {
//								if (i == 0) {
//									debugView.outputText.text = debugView.logs [i];
//								} else {
//									debugView.outputText.text += Environment.NewLine + debugView.logs [i];
//								}
//							}
//						}
						debugView.Refresh = false;
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);
				}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer);
			}).AddTo (this.Disposer);
		}

		void Start ()
		{
		}

		public void Refresh ()
		{
			var masks = DebugMask.ToList ();
			var layers = new ReactiveCollection<string> ();
			for (int i = 0; i < masks.Count; i++) {
				if (masks [i].isEnable) {
					layers.Add (masks [i].layerName);
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

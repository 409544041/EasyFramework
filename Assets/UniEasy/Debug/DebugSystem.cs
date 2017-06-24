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
					debugView.outputArea.content = debugView.outputPanel;

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

					CommandLibrary.RegisterCommand ("Debug", "Show debug console message on UGui.", "Debug [On/Off]", (args) => {
						if (args.Length == 0) {
							return "Sorry, it is a invalid command!";
						} else {
							var command = args [0].ToLower ();
							var result = command.Equals ("on") ? true : command.Equals ("true") ? true : false;
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
						while (debugView.logs.Count > 0) {
							Destroy (debugView.logs [0].GameObject);
							debugView.logs.RemoveAt (0);
						}
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.collapseToggle = CreateToggle ("Collapse", debugView.menuPanel.transform, out debugView.collapseText);
					debugView.collapseToggle.transform.ToRectTransform (Vector2.zero, new Vector2 (0, 1), new Vector2 (60, 0), new Vector2 (120, 0));
					debugView.collapseToggle.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.Collapse = debugView.collapseToggle.isOn;
						DebugWriter.Set ("collapse", debugView.Collapse);
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.logToggle = CreateToggle ("Log", debugView.menuPanel.transform, out debugView.logText);
					debugView.logToggle.transform.ToRectTransform (new Vector2 (1, 0), Vector2.one, new Vector2 (80, 0), new Vector2 (-190, 0));
					debugView.logToggle.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.Log = debugView.logToggle.isOn;
						DebugWriter.Set ("log", debugView.Log);
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.warningToggle = CreateToggle ("Warning", debugView.menuPanel.transform, out debugView.warningText);
					debugView.warningToggle.transform.ToRectTransform (new Vector2 (1, 0), Vector2.one, new Vector2 (80, 0), new Vector2 (-105, 0));
					debugView.warningToggle.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.Warning = debugView.warningToggle.isOn;
						DebugWriter.Set ("warning", debugView.Warning);
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.errorToggle = CreateToggle ("Error", debugView.menuPanel.transform, out debugView.errorText);
					debugView.errorToggle.transform.ToRectTransform (new Vector2 (1, 0), Vector2.one, new Vector2 (60, 0), new Vector2 (-30, 0));
					debugView.errorToggle.OnPointerClickAsObservable ().Subscribe (_ => {
						debugView.Error = debugView.errorToggle.isOn;
						DebugWriter.Set ("error", debugView.Error);
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
							var log = new DebugLog (type, debugView.outputPanel);
							log.Message = message.ToString ();
							log.Times = 1;
							debugView.logs.Add (log);
						}
					};
						
					var onCollapse = debugView.collapse.DistinctUntilChanged ().AsObservable ();
					var onLog = debugView.log.DistinctUntilChanged ().AsObservable ();
					var onWarning = debugView.warning.DistinctUntilChanged ().AsObservable ();
					var onError = debugView.error.DistinctUntilChanged ().AsObservable ();
					var onAdd = debugView.logs.ObserveAdd ().Select (x => x.Value).AsObservable ();
					var onRemove = debugView.logs.ObserveRemove ().Select (x => x.Value).AsObservable ();
					onCollapse.Merge (onLog).Merge (onWarning).Merge (onError).CombineLatest (onAdd.Merge (onRemove), (x, y) => true).Subscribe (_ => {
						var list = new ReactiveDictionary<string, DebugLog> ();
						for (int i = 0; i < debugView.logs.Count; i++) {
							var log = debugView.logs [i];
							if (i % 2 == 0) {
								log.outputBackground.color = new Color32 (0x00, 0x00, 0x00, 0x24);
								log.outputTimesBackground.color = new Color32 (0x42, 0x42, 0x42, 0x80);
							} else {
								log.outputBackground.color = new Color32 (0x00, 0x00, 0x00, 0x48);
								log.outputTimesBackground.color = new Color32 (0xA9, 0xA9, 0xA9, 0x80);
							}
							if (debugView.Collapse) {
								if (list.ContainsKey (log.Message)) {
									log.GameObject.SetActive (false);
									var t = list [log.Message];
									t.Times++;
									list [log.Message] = t;
								} else {
									log.GameObject.SetActive (true);
									log.Times = 1;
									list.Add (log.Message, log);
								}
							} else {
								log.GameObject.SetActive (true);
								log.Times = 1;
							}
							if (log.type == LogType.Log && !debugView.Log) {
								log.GameObject.SetActive (false);
							} else if (log.type == LogType.Warning && !debugView.Warning) {
								log.GameObject.SetActive (false);
							} else if (log.type == LogType.Error && !debugView.Error) {
								log.GameObject.SetActive (false);
							}
						}

						var activeLogs = debugView.logs.Where (x => x.GameObject.activeSelf).Select (x => x.type);
						debugView.logText.text = string.Format ("Log ({0})", activeLogs.Where (x => x == LogType.Log).Count ());
						debugView.warningText.text = string.Format ("Warning ({0})", activeLogs.Where (x => x == LogType.Warning).Count ());
						debugView.errorText.text = string.Format ("Error ({0})", activeLogs.Where (x => x == LogType.Error).Count ());
					}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer).AddTo (debugView.Disposer);

					debugView.collapseToggle.isOn = DebugWriter.HasKey ("collapse") ? DebugWriter.Get<bool> ("collapse") : true;
					debugView.logToggle.isOn = DebugWriter.HasKey ("log") ? DebugWriter.Get<bool> ("log") : true;
					debugView.warningToggle.isOn = DebugWriter.HasKey ("warning") ? DebugWriter.Get<bool> ("warning") : true;
					debugView.errorToggle.isOn = DebugWriter.HasKey ("error") ? DebugWriter.Get<bool> ("error") : true;

					debugView.panel.gameObject.SetActive (false);
				}).AddTo (this.Disposer).AddTo (debugCanvas.Disposer);
			}).AddTo (this.Disposer);
		}

		void Start ()
		{
		}

		public Toggle CreateToggle (string name, Transform parent, out Text text)
		{
			var toggle = UIUtility.CreateToggle (name, parent);
			toggle.graphic.color = new Color32 (0xCC, 0xCC, 0xCC, 0xFF);
			text = UIUtility.Create<Text> (string.Format ("{0}Text", name), toggle.transform);
			text.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
			text.ToConfigure (new Color32 (0x20, 0x20, 0x20, 0xFF), alignment : TextAnchor.MiddleCenter, fontSize : 12);
			text.text = name;
			return toggle;
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

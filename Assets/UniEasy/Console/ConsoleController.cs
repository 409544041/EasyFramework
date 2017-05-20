using UnityEngine.UI;
using UniRx.Triggers;
using UniEasy.ECS;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

namespace UniEasy.Console
{
	public class ConsoleController : SystemBehaviour
	{
		private const int inputHistoryCapacity = 100;
		private ConsoleInputHistory inputHistory = new ConsoleInputHistory (inputHistoryCapacity);

		public override void Setup ()
		{
			base.Setup ();

			var group = GroupFactory.Create (new Type[] {
				typeof(EntityBehaviour),
				typeof(ConsoleView),
			});

			group.Entities.ObserveAdd ().Select (x => x.Value).StartWith (group.Entities).Subscribe (entity => {
				var consoleView = entity.GetComponent<ConsoleView> ();

				consoleView.canvas = (Canvas)CreateUIComponent ("Canvas", consoleView.transform.parent, typeof(Canvas));
				consoleView.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				consoleView.canvasScaler = consoleView.canvas.gameObject.AddComponent<CanvasScaler> ();
				consoleView.graphicRaycaster = consoleView.canvas.gameObject.AddComponent<GraphicRaycaster> ();

				consoleView.panel = (RectTransform)CreateUIComponent ("Console Panel", consoleView.canvas.transform, typeof(RectTransform));
				ConfigureRectTransform (consoleView.panel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

				consoleView.outputArea = (ScrollRect)CreateUIComponent ("OutputArea", consoleView.panel.transform, typeof(ScrollRect));
				ConfigureRectTransform (consoleView.outputArea.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.viewport = (RectTransform)CreateUIComponent ("Viewport", consoleView.outputArea.transform, typeof(RectTransform));
				ConfigureRectTransform (consoleView.outputArea.viewport.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.viewport.gameObject.AddComponent<Image> ().raycastTarget = false;
				consoleView.outputArea.viewport.gameObject.AddComponent<Mask> ().showMaskGraphic = false;
				var scrollbar = (Image)CreateUIComponent ("Scrollbar Vertical", consoleView.outputArea.transform, typeof(Image));
				scrollbar.color = new Color32 (0x20, 0x20, 0x20, 0xFF);
				consoleView.outputArea.horizontal = false;
				consoleView.outputArea.verticalScrollbar = scrollbar.gameObject.AddComponent<Scrollbar> ();
				consoleView.scrollbar = consoleView.outputArea.verticalScrollbar;
				consoleView.scrollbar.direction = Scrollbar.Direction.BottomToTop;
				consoleView.scrollbar.targetGraphic = scrollbar;
				consoleView.outputArea.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
				consoleView.outputArea.verticalScrollbarSpacing = -3;
				ConfigureRectTransform (consoleView.scrollbar.transform, new Vector2 (1, 0), Vector2.one, new Vector2 (20, 0), new Vector2 (-10, 0));
				var slidingArea = (RectTransform)CreateUIComponent ("Sliding Area", consoleView.scrollbar.transform, typeof(RectTransform));
				ConfigureRectTransform (slidingArea, Vector2.zero, Vector2.one, new Vector2 (-20, -20), Vector2.zero);
				var handle = (Image)CreateUIComponent ("Handle", slidingArea, typeof(Image));
				ConfigureRectTransform (handle.transform, Vector2.zero, Vector2.one, new Vector2 (20, 20), Vector2.zero);
				consoleView.scrollbar.targetGraphic = handle;
				consoleView.scrollbar.handleRect = handle.rectTransform;
				consoleView.outputText = (Text)CreateUIComponent ("OutputText", consoleView.outputArea.viewport.transform, typeof(Text));
				consoleView.outputText.gameObject.AddComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				ConfigureText (consoleView.outputText, new Color32 (0x32, 0x32, 0x32, 0xFF), raycastTarget: false);
				ConfigureRectTransform (consoleView.outputText.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.content = (RectTransform)consoleView.outputText.transform;

				var bg = (Image)CreateUIComponent ("InputField", consoleView.panel.transform, typeof(Image));
				var placeholder = (Text)CreateUIComponent ("Placeholder", bg.transform, typeof(Text));
				var inputText = (Text)CreateUIComponent ("InputText", bg.transform, typeof(Text));
				bg.color = new Color32 (0x00, 0x00, 0x00, 0x80);
				consoleView.inputField = bg.gameObject.AddComponent<InputField> ();
				consoleView.inputField.targetGraphic = bg;
				consoleView.inputField.placeholder = placeholder;
				consoleView.inputField.textComponent = inputText;
				ConfigureText (placeholder, new Color32 (0xFF, 0xFF, 0xFF, 0x80));
				ConfigureText (inputText, new Color32 (0xFF, 0xFF, 0xFF, 0xFF), supportRichText: false);
				ConfigureRectTransform (bg.transform, Vector2.zero, new Vector2 (1, 0), new Vector2 (0, 20), new Vector2 (0, 10));
				ConfigureRectTransform (placeholder.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				ConfigureRectTransform (inputText.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

				consoleView.inputField.OnSubmitAsObservable ().Subscribe (e => {
					if (UnityEngine.EventSystems.EventSystem.current.alreadySelecting)
						return;
					if (consoleView.inputField.text.Length > 0) {
						ExecuteCommand (consoleView.inputField.text);
						consoleView.scrollbar.value = 0;
						consoleView.outputText.text += string.IsNullOrEmpty (consoleView.outputText.text) ? consoleView.inputField.text : Environment.NewLine + consoleView.inputField.text;
						consoleView.inputField.MoveTextStart (false);
						consoleView.inputField.text = "";
						consoleView.inputField.MoveTextEnd (false);
					}
					consoleView.inputField.ActivateInputField ();
				}).AddTo (this.Disposer).AddTo (consoleView.Disposer);

				var clickStream = Observable.EveryUpdate ().Where (_ => Input.anyKeyDown);

				clickStream.Subscribe (_ => {
					if (Input.GetKeyDown (KeyCode.BackQuote)) {
						consoleView.panel.gameObject.SetActive (!consoleView.panel.gameObject.activeSelf);
					} else if (Input.GetKeyDown (KeyCode.Escape)) {
						consoleView.panel.gameObject.SetActive (false);
					} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
						var navigatedToInput = inputHistory.Navigate (true);
						consoleView.inputField.MoveTextStart (false);
						consoleView.inputField.text = navigatedToInput;
						consoleView.inputField.MoveTextEnd (false);
					} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
						var navigatedToInput = inputHistory.Navigate (false);
						consoleView.inputField.MoveTextStart (false);
						consoleView.inputField.text = navigatedToInput;
						consoleView.inputField.MoveTextEnd (false);
					}
				}).AddTo (this.Disposer).AddTo (consoleView.Disposer);

				consoleView.panel.gameObject.SetActive (false);
			}).AddTo (this.Disposer);
		}

		Component CreateUIComponent (string name, Transform parent, System.Type type)
		{
			var go = new GameObject (name);
			go.transform.SetParent (parent);
			go.layer = LayerMask.NameToLayer ("UI");
			return go.AddComponent (type);
		}

		Text ConfigureText (Text text, Color32 color, string fontName = "Arial", int fontSize = 14, FontStyle fontStyle = FontStyle.Normal, TextAnchor alignment = TextAnchor.UpperLeft, bool supportRichText = true, bool raycastTarget = true)
		{
			text.font = Font.CreateDynamicFontFromOSFont (fontName, fontSize);
			text.supportRichText = supportRichText;
			text.raycastTarget = raycastTarget;
			text.fontStyle = fontStyle;
			text.alignment = alignment;
			text.color = color;
			return text;
		}

		RectTransform ConfigureRectTransform (Transform transform, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition)
		{
			var rectTransform = (RectTransform)transform;
			rectTransform.anchorMax = anchorMax;
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchoredPosition = anchoredPosition;
			rectTransform.sizeDelta = sizeDelta;
			return rectTransform;
		}

		void ExecuteCommand (string input)
		{
			string[] parts = input.Split (' ');
			string command = parts [0];
			string[] args = parts.Skip (1).ToArray ();

			Console.Log ("> " + input);
			Console.Log (CommandLibrary.ExecuteCommand (command, args));
			inputHistory.AddNewInputEntry (input);
		}
	}
}

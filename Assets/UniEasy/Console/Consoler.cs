using UnityEngine.UI;
using UniRx.Triggers;
using UniEasy.ECS;
using UnityEngine;
using System.Linq;
using System;
using UniRx;

namespace UniEasy.Console
{
	public class Consoler : SystemBehaviour
	{
		private const int inputHistoryCapacity = 100;
		private ConsoleInputHistory inputHistory = new ConsoleInputHistory (inputHistoryCapacity);
		private ConsoleView consoleView;

		public override void Setup ()
		{
			base.Setup ();

			var debugCanvasEntities = GroupFactory.Create (typeof(DebugCanvas));
			var group = GroupFactory.Create (typeof(ConsoleView));

			group.OnAdd ().Subscribe (entity => {
				consoleView = entity.GetComponent<ConsoleView> ();

				consoleView.panel = CreateUI<RectTransform> ("ConsolePanel", debugCanvasEntities.Entities.Select (x => x.GetComponent<DebugCanvas> ().transform).FirstOrDefault ());
				ConfigureRectTransform (consoleView.panel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

				consoleView.outputArea = CreateUI<ScrollRect> ("OutputArea", consoleView.panel.transform);
				ConfigureRectTransform (consoleView.outputArea.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.viewport = CreateUI<RectTransform> ("Viewport", consoleView.outputArea.transform);
				ConfigureRectTransform (consoleView.outputArea.viewport.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.viewport.gameObject.AddComponent<Image> ().raycastTarget = false;
				consoleView.outputArea.viewport.gameObject.AddComponent<Mask> ().showMaskGraphic = false;
				var scrollbar = CreateUI<Image> ("ScrollbarVertical", consoleView.outputArea.transform);
				scrollbar.color = new Color32 (0x20, 0x20, 0x20, 0xFF);
				consoleView.outputArea.horizontal = false;
				consoleView.outputArea.verticalScrollbar = scrollbar.gameObject.AddComponent<Scrollbar> ();
				consoleView.scrollbar = consoleView.outputArea.verticalScrollbar;
				consoleView.scrollbar.direction = Scrollbar.Direction.BottomToTop;
				consoleView.scrollbar.targetGraphic = scrollbar;
				consoleView.outputArea.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
				consoleView.outputArea.verticalScrollbarSpacing = -3;
				ConfigureRectTransform (consoleView.scrollbar.transform, new Vector2 (1, 0), Vector2.one, new Vector2 (20, 0), new Vector2 (-10, 0));
				var slidingArea = CreateUI<RectTransform> ("SlidingArea", consoleView.scrollbar.transform);
				ConfigureRectTransform (slidingArea, Vector2.zero, Vector2.one, new Vector2 (-20, -20), Vector2.zero);
				var handle = CreateUI<Image> ("Handle", slidingArea);
				ConfigureRectTransform (handle.transform, Vector2.zero, Vector2.one, new Vector2 (20, 20), Vector2.zero);
				consoleView.scrollbar.targetGraphic = handle;
				consoleView.scrollbar.handleRect = handle.rectTransform;
				consoleView.outputText = CreateUI<Text> ("OutputText", consoleView.outputArea.viewport.transform);
				consoleView.outputText.gameObject.AddComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				ConfigureText (consoleView.outputText, new Color32 (0x32, 0x32, 0x32, 0xFF), raycastTarget: false);
				ConfigureRectTransform (consoleView.outputText.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.content = (RectTransform)consoleView.outputText.transform;

				var bg = CreateUI<Image> ("InputField", consoleView.panel.transform);
				var placeholder = CreateUI<Text> ("Placeholder", bg.transform);
				var inputText = CreateUI<Text> ("InputText", bg.transform);
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
						if (consoleView.panel.gameObject.activeSelf) {
							consoleView.inputField.ActivateInputField ();
						}
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

		T CreateUI<T> (string name, Transform parent) where T : Component
		{
			var go = new GameObject (name);
			go.transform.SetParent (parent);
			go.layer = LayerMask.NameToLayer ("UI");
			return go.AddComponent<T> ();
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

		public void ExecuteCommand (string input)
		{
			var parts = input.Split (' ');
			var command = parts [0];
			var args = parts.Skip (1).ToArray ();
			
			Log ("> " + input);
			Log (CommandLibrary.ExecuteCommand (command, args));
			inputHistory.AddNewInputEntry (input);
		}

		public void Log (string message)
		{
			#if UNITY_EDITOR
			Debug.Log (message);
			#endif
			if (consoleView != null) {
				if (string.IsNullOrEmpty (consoleView.outputText.text)) {
					consoleView.outputText.text += message;
				} else {
					consoleView.outputText.text += Environment.NewLine + message;
				}
			}
		}
	}
}

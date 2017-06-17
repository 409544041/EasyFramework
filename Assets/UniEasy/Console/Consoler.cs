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

				consoleView.panel = UIUtility.Create<RectTransform> ("ConsolePanel", debugCanvasEntities.Entities.Select (x => x.GetComponent<DebugCanvas> ().transform).FirstOrDefault ());
				consoleView.panel.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

				consoleView.outputArea = UIUtility.Create<ScrollRect> ("OutputArea", consoleView.panel.transform);
				consoleView.outputArea.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.viewport = UIUtility.Create<RectTransform> ("Viewport", consoleView.outputArea.transform);
				consoleView.outputArea.viewport.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.viewport.gameObject.AddComponent<Image> ().raycastTarget = false;
				consoleView.outputArea.viewport.gameObject.AddComponent<Mask> ().showMaskGraphic = false;
				var scrollbar = UIUtility.Create<Image> ("ScrollbarVertical", consoleView.outputArea.transform);
				scrollbar.color = new Color32 (0x20, 0x20, 0x20, 0xFF);
				consoleView.outputArea.horizontal = false;
				consoleView.outputArea.verticalScrollbar = scrollbar.gameObject.AddComponent<Scrollbar> ();
				consoleView.scrollbar = consoleView.outputArea.verticalScrollbar;
				consoleView.scrollbar.direction = Scrollbar.Direction.BottomToTop;
				consoleView.scrollbar.targetGraphic = scrollbar;
				consoleView.outputArea.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
				consoleView.outputArea.verticalScrollbarSpacing = -3;
				consoleView.scrollbar.transform.ToRectTransform (new Vector2 (1, 0), Vector2.one, new Vector2 (20, 0), new Vector2 (-10, 0));
				var slidingArea = UIUtility.Create<RectTransform> ("SlidingArea", consoleView.scrollbar.transform);
				slidingArea.ToRectTransform (Vector2.zero, Vector2.one, new Vector2 (-20, -20), Vector2.zero);
				var handle = UIUtility.Create<Image> ("Handle", slidingArea);
				handle.transform.ToRectTransform (Vector2.zero, Vector2.one, new Vector2 (20, 20), Vector2.zero);
				consoleView.scrollbar.targetGraphic = handle;
				consoleView.scrollbar.handleRect = handle.rectTransform;
				consoleView.outputText = UIUtility.Create<Text> ("OutputText", consoleView.outputArea.viewport.transform);
				consoleView.outputText.gameObject.AddComponent<ContentSizeFitter> ().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				consoleView.outputText.ToConfigure (new Color32 (0x32, 0x32, 0x32, 0xFF), raycastTarget: false);
				consoleView.outputText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				consoleView.outputArea.content = (RectTransform)consoleView.outputText.transform;

				var bg = UIUtility.Create<Image> ("InputField", consoleView.panel.transform);
				var placeholder = UIUtility.Create<Text> ("Placeholder", bg.transform);
				var inputText = UIUtility.Create<Text> ("InputText", bg.transform);
				bg.color = new Color32 (0x00, 0x00, 0x00, 0x80);
				consoleView.inputField = bg.gameObject.AddComponent<InputField> ();
				consoleView.inputField.targetGraphic = bg;
				consoleView.inputField.placeholder = placeholder;
				consoleView.inputField.textComponent = inputText;
				placeholder.ToConfigure (new Color32 (0xFF, 0xFF, 0xFF, 0x80));
				inputText.ToConfigure (new Color32 (0xFF, 0xFF, 0xFF, 0xFF), supportRichText: false);
				bg.transform.ToRectTransform (Vector2.zero, new Vector2 (1, 0), new Vector2 (0, 20), new Vector2 (0, 10));
				placeholder.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
				inputText.transform.ToRectTransform (Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

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

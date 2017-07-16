using UnityEngine.UI;
using UnityEngine;
using UniEasy.ECS;

namespace UniEasy.Console
{
	public class ConsoleView : ComponentBehaviour
	{
		public RectTransform panel;
		public InputField inputField;
		[HideInInspector]
		public Text outputText;
		[HideInInspector]
		public Scrollbar scrollbar;
		[HideInInspector]
		public ScrollRect outputArea;
		public Button cancelButton;
		public Button okButton;
	}
}

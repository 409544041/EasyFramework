using UnityEngine.UI;
using UnityEngine;
using UniEasy.ECS;

namespace UniEasy.Console
{
	public class ConsoleView : ComponentBehaviour
	{
		public Canvas canvas;
		public Text outputText;
		public Scrollbar scrollbar;
		public RectTransform panel;
		public ScrollRect outputArea;
		public InputField inputField;
		public CanvasScaler canvasScaler;
		public GraphicRaycaster graphicRaycaster;
	}
}

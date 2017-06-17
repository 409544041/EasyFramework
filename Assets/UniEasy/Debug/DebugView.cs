using UnityEngine.UI;
using UnityEngine;

namespace UniEasy.ECS
{
	public class DebugView : ComponentBehaviour
	{
		public RectTransform panel;
		public int size = 14;
		public Text outputText;
		public Scrollbar scrollbar;
		public ScrollRect outputArea;
	}
}

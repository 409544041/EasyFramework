using UnityEngine.UI;
using UnityEngine;
using UniRx;

namespace UniEasy.ECS
{
	public class DebugView : ComponentBehaviour
	{
		public RectTransform panel;
		public int size = 14;
		public Text outputText;
		public Text collapseCountText;
		public Scrollbar scrollbar;
		public ScrollRect outputArea;
		public RectTransform menuPanel;
		public Button escButton;
		public Button clearButton;
		public Toggle collapseToggle;

		[HideInInspector]
		public bool collapse;
		[HideInInspector]
		public BoolReactiveProperty refresh = new BoolReactiveProperty ();
		public ReactiveCollection<string> logs = new ReactiveCollection<string> ();
//		public ReactiveCollection<Image> backgrouds = new ReactiveCollection<Image> ();
		public ReactiveDictionary<string, int> collapseLogs = new ReactiveDictionary<string, int> ();

		public bool Refresh {
			get {
				return refresh.Value;
			}
			set {
				refresh.Value = value;
			}
		}
	}
}

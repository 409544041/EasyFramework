using UnityEngine.UI;
using UniEasy.ECS;
using UnityEngine;
using UniRx;

namespace UniEasy.Console
{
	public class DebugView : ComponentBehaviour
	{
		public RectTransform panel;
		public int size = 14;
		public RectTransform outputPanel;
		public VerticalLayoutGroup outputLayout;
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
		public ReactiveCollection<DebugLine> lines = new ReactiveCollection<DebugLine> ();
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

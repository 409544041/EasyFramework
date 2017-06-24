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
		public BoolReactiveProperty collapse = new BoolReactiveProperty ();
		public ReactiveCollection<DebugLog> logs = new ReactiveCollection<DebugLog> ();

		public bool Collapse {
			get {
				return collapse.Value;
			}
			set {
				collapse.Value = value;
			}
		}
	}
}

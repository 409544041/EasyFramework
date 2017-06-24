using UnityEngine.UI;
using UniEasy.ECS;
using UnityEngine;
using UniRx;

namespace UniEasy.Console
{
	public class DebugView : ComponentBehaviour
	{
		[HideInInspector]
		public RectTransform panel;
		public readonly int size = 14;
		public RectTransform outputPanel;
		[HideInInspector]
		public VerticalLayoutGroup outputLayout;
		[HideInInspector]
		public Scrollbar scrollbar;
		[HideInInspector]
		public ScrollRect outputArea;
		[HideInInspector]
		public RectTransform menuPanel;
		public Button escButton;
		public Button clearButton;
		public Toggle collapseToggle;
		public Toggle logToggle;
		public Toggle warningToggle;
		public Toggle errorToggle;
		[HideInInspector]
		public Text collapseText;
		[HideInInspector]
		public Text logText;
		[HideInInspector]
		public Text warningText;
		[HideInInspector]
		public Text errorText;
		public ReactiveProperty<bool> collapse = new ReactiveProperty<bool> (true);
		public ReactiveProperty<bool> log = new ReactiveProperty<bool> (true);
		public ReactiveProperty<bool> warning = new ReactiveProperty<bool> (true);
		public ReactiveProperty<bool> error = new ReactiveProperty<bool> (true);
		public ReactiveCollection<DebugLog> logs = new ReactiveCollection<DebugLog> ();

		public bool Collapse {
			get {
				return collapse.Value;
			}
			set {
				collapse.Value = value;
			}
		}

		public bool Log {
			get {
				return log.Value;
			}
			set {
				log.Value = value;
			}
		}

		public bool Warning {
			get {
				return warning.Value;
			}
			set {
				warning.Value = value;
			}
		}

		public bool Error {
			get {
				return error.Value;
			}
			set {
				error.Value = value;
			}
		}
	}
}

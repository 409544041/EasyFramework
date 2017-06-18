using UnityEngine;
using UnityEditor;
using UniEasy.ECS;

namespace UniEasy.Edit
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(DebugSystem))]
	public class DebugSystemInspector : Editor
	{
		private bool isDirty;

		public bool IsDirty {
			get {
				return isDirty;
			}
			set {
				isDirty = isDirty == true ? true : value;
			}
		}

		private DebugSystem DebugSystem { get; set; }

		void OnEnable ()
		{
			this.DebugSystem = target as DebugSystem;
		}

		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginVertical ();
			isDirty = false;
 
			var isLogEnable = EditorGUILayout.ToggleLeft ("IsLogEnabled", this.DebugSystem.DebugMask.IsLogEnabled);
			IsDirty = !(isLogEnable == this.DebugSystem.DebugMask.IsLogEnabled);

			var showOnUGUI = false;
			var masks = this.DebugSystem.DebugMask.ToList ();
			if (isLogEnable) {
				showOnUGUI = EditorGUILayout.ToggleLeft ("Show On UGUI", DebugSystem.ShowOnUGUI);
				IsDirty = !(showOnUGUI == DebugSystem.ShowOnUGUI);
				for (int i = 0; i < masks.Count; i++) {
					EditorGUILayout.BeginHorizontal ();
					var IsEnable = EditorGUILayout.Toggle (masks [i].IsEnable, GUILayout.MinWidth (12), GUILayout.MaxWidth (24));
					var LayerName = EditorGUILayout.TextField (masks [i].LayerName);
					IsDirty = !(IsEnable == masks [i].IsEnable);
					IsDirty = !(LayerName == masks [i].LayerName);
					masks [i] = new DebugLayer (IsEnable, LayerName);
					if (GUILayout.Button ("-", GUILayout.MinWidth (20), GUILayout.MaxWidth (20))) {
						masks.RemoveAt (i);
						IsDirty = true;
						break;
					}
					EditorGUILayout.EndHorizontal ();
				}
				if (GUILayout.Button ("Add Layer...")) {
					masks.Add (new DebugLayer (true, "Layer " + masks.Count));
					IsDirty = true;
				}
			}
			if (IsDirty) {
				this.DebugSystem.DebugMask = new DebugMask (masks);
				this.DebugSystem.DebugMask.IsLogEnabled = isLogEnable;
				this.DebugSystem.ShowOnUGUI = showOnUGUI;
				this.DebugSystem.Save ();
				if (Application.isPlaying) {
					this.DebugSystem.Refresh ();
				}
			}
			EditorGUILayout.EndVertical ();
		}
	}
}

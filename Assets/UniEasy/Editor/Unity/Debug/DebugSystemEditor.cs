using UnityEngine;
using UnityEditor;
using UniEasy.ECS;

namespace UniEasy.Edit
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(DebugSystem))]
	public class DebugSystemEditor : Editor
	{
		private DebugSystem DebugSystem;
		private bool isDirty;

		void OnEnable ()
		{
			this.DebugSystem = target as DebugSystem;
		}

		public override void OnInspectorGUI ()
		{
			EditorGUILayout.BeginVertical ();
			isDirty = false;
			var masks = this.DebugSystem.DebugMask.ToList ();
			for (int i = 0; i < masks.Count; i++) {
				EditorGUILayout.BeginHorizontal ();
				var IsEnable = EditorGUILayout.Toggle (masks [i].IsEnable, GUILayout.MinWidth (12), GUILayout.MaxWidth (24));
				var LayerName = EditorGUILayout.TextField (masks [i].LayerName);
				isDirty = isDirty == false ? !(IsEnable == masks [i].IsEnable) : true;
				isDirty = isDirty == false ? !(LayerName == masks [i].LayerName) : true;
				masks [i] = new DebugLayer (IsEnable, LayerName);
				if (GUILayout.Button ("-", GUILayout.MinWidth (20), GUILayout.MaxWidth (20))) {
					masks.RemoveAt (i);
					isDirty = true;
					break;
				}
				EditorGUILayout.EndHorizontal ();
			}
			if (GUILayout.Button ("Add Layer...")) {
				masks.Add (new DebugLayer (true, "Layer " + masks.Count));
				isDirty = true;
			}
			if (isDirty) {
				this.DebugSystem.DebugMask = new DebugMask (masks);
				this.DebugSystem.Dispose ();
				if (Application.isPlaying) {
					this.DebugSystem.Refresh ();
				}
			}
			EditorGUILayout.EndVertical ();
		}
	}
}

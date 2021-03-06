﻿using UniEasy.Console;
using UnityEngine;
using UnityEditor;

namespace UniEasy.Edit
{
	[CanEditMultipleObjects]
	[CustomEditor (typeof(DebugSystem))]
	public class DebugSystemDrawer : Editor
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
			DebugSystem = target as DebugSystem;
		}

		public override void OnInspectorGUI ()
		{
			if (DebugSystem.DebugMask == null) {
				return;
			}

			EditorGUILayout.BeginVertical ();
			isDirty = false;

			var isLogEnable = EditorGUILayout.ToggleLeft ("IsLogEnabled", DebugSystem.IsLogEnabled);
			IsDirty = !(isLogEnable == DebugSystem.IsLogEnabled);

			var showOnUGUI = false;
			var masks = DebugSystem.DebugMask.ToList ();
			if (isLogEnable) {
				showOnUGUI = EditorGUILayout.ToggleLeft ("Show On UGUI", DebugSystem.ShowOnUGUI);
				IsDirty = !(showOnUGUI == DebugSystem.ShowOnUGUI);
				for (int i = 0; i < masks.Count; i++) {
					EditorGUILayout.BeginHorizontal ();
					var IsEnable = EditorGUILayout.Toggle (masks [i].isEnable, GUILayout.MinWidth (12), GUILayout.MaxWidth (24));
					var LayerName = EditorGUILayout.TextField (masks [i].layerName);
					IsDirty = !(IsEnable == masks [i].isEnable);
					IsDirty = !(LayerName == masks [i].layerName);
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
				DebugSystem.IsLogEnabled = isLogEnable;
				DebugSystem.ShowOnUGUI = showOnUGUI;
				DebugSystem.DebugMask = new DebugMask (masks);
				DebugSystem.Record ();
				if (Application.isPlaying) {
					DebugSystem.Refresh ();
				}
			}
			EditorGUILayout.EndVertical ();
		}
	}
}

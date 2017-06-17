using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System;

namespace UniEasy.Edit
{
	/// <summary>
	/// Create ScriptableObject Assets Window.
	/// </summary>
	public class ScriptableObjectWindow : EditorWindow
	{
		private int selectedNamespaceIndex;
		private int selectedTypeIndex;
		private string searchCondition = "";
		private Type currentSelectedType;
		private UnityEngine.Object currentSelectedObject;
		static private Type[] scriptableTypes;
		static private Dictionary<string, Type[]> scriptableDictionary = new Dictionary<string, Type[]> ();

		[MenuItem ("Assets/Create/UniEasy/ScriptableObject Window", false, 62)]
		public static void OpenScriptableObjectWindow ()
		{
			Steup ();

			var window = EditorWindow.GetWindow<ScriptableObjectWindow> (false, "Scriptable", true);
			window.ShowPopup ();
		}

		static void Steup ()
		{
			// Get all classes derived from ScriptableObject
			scriptableTypes = AssemblyHelper.CSharp.GetTypes ().Where (t => t.IsSubclassOf (typeof(ScriptableObject)) && !t.IsGenericType).Distinct ().ToArray ();
			scriptableDictionary = scriptableTypes.Select (t => t.Namespace).Distinct ().ToDictionary (k => k, v => scriptableTypes.Where (t => t.Namespace == v).ToArray ());
		}

		void OnEnable ()
		{
			Steup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("Create a ScriptableObject Asset");
			// Search
			EditorGUILayout.BeginHorizontal ();
			GUIStyle searchStyle = (GUIStyle)"SearchTextField";
			GUIStyle cancelStyle = (GUIStyle)"SearchCancelButton";
			GUIStyle cancelEmptyStyle = (GUIStyle)"SearchCancelButtonEmpty";
			searchCondition = GUILayout.TextField (searchCondition, searchStyle);
			if (!string.IsNullOrEmpty (searchCondition)) {
				if (GUILayout.Button ("", cancelStyle)) {
					searchCondition = "";
				}
			} else {
				GUILayout.Button ("", cancelEmptyStyle);
			}
			EditorGUILayout.EndHorizontal ();
			// Select depend on namespace
			string[] keys;
			if (!string.IsNullOrEmpty (searchCondition)) {
				var key0 = scriptableTypes.Where (t => t.FullName.Length >= searchCondition.Length && t.FullName.StartsWith (searchCondition, true, null)).Select (v => v.Namespace);
				var key1 = scriptableTypes.Where (t => t.Namespace.Length >= searchCondition.Length && t.Namespace.StartsWith (searchCondition, true, null)).Select (v => v.Namespace);
				var key2 = scriptableTypes.Where (t => t.Name.Length >= searchCondition.Length && t.Name.StartsWith (searchCondition, true, null)).Select (v => v.Namespace);
				keys = key0.Union (key1).Union (key2).Distinct ().ToArray ();
			} else {
				keys = scriptableTypes.Select (t => t.Namespace).Distinct ().ToArray ();
			}

			// If can not search any scriptableobject Namespace
			if (keys == null || keys.Length <= 0) {
				return;
			}

			selectedNamespaceIndex = Mathf.Clamp (selectedNamespaceIndex, 0, keys.Length - 1);
			selectedNamespaceIndex = EditorGUILayout.Popup (selectedNamespaceIndex, keys);
			var currentKey = keys [selectedNamespaceIndex];
			// Select scriptableobject class
			string[] names;
			if (!string.IsNullOrEmpty (searchCondition)) {
				var types = scriptableDictionary [currentKey];
				var name0 = types.Where (t => t.FullName.Length >= searchCondition.Length && t.FullName.StartsWith (searchCondition, true, null)).Select (v => v.Name);
				var name1 = types.Where (t => t.Name.Length >= searchCondition.Length && t.Name.StartsWith (searchCondition, true, null)).Select (v => v.Name);
				names = name0.Union (name1).ToArray ();
			} else {
				names = scriptableDictionary [currentKey].Select (t => t.Name).ToArray ();
			}
			// If can not search any scriptableobject Name
			if (names == null || names.Length <= 0) {
				return;
			}

			selectedTypeIndex = Mathf.Clamp (selectedTypeIndex, 0, names.Length - 1);
			selectedTypeIndex = EditorGUILayout.Popup (selectedTypeIndex, names);

			var type = scriptableDictionary [currentKey].Where (t => t.Name == names [selectedTypeIndex]).FirstOrDefault ();
			var isDirty = currentSelectedType == type ? false : true;

			if (isDirty) {
				var guids = AssetDatabase.FindAssets ("t:Object");
				var objs = guids.Select (guid => AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript> (AssetDatabase.GUIDToAssetPath (guid))).Where (o => o != null);
				currentSelectedObject = objs.Where (o => o != null && o.GetClass () == type).Select (o => o as UnityEngine.Object).FirstOrDefault ();
				currentSelectedType = type;
			}
			EditorGUILayout.ObjectField (currentSelectedObject, typeof(UnityEditor.MonoScript), false);

			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Create")) {
				ScriptableObjectInstallerUtility.Create (names [selectedTypeIndex], type);
			}
		}
	}
}

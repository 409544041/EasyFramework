using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;

namespace UniEasy.Edit
{
	/// <summary>
	/// Create Template Script Assets Window.
	/// </summary>
	public class ScriptAssetWindow : EditorWindow
	{
		private int selectedTypeIndex = 0;
		private int selectedNamespaceIndex = 0;
		private string searchCondition = "";
		private Vector2 scrollPos = Vector2.zero;
		private Type currentSelectedType;
		private UnityEngine.Object currentSelectedObject;
		static private Type[] monoScriptTypes;
		static private Dictionary<string, Type[]> monoScriptDictionary = new Dictionary<string, Type[]> ();
		static private Dictionary<Type, IScriptAssetInstaller> installers = new Dictionary<Type, IScriptAssetInstaller> ();

		[MenuItem ("Assets/Create/UniEasy/Template Script Window", false, 61)]
		static public void OpenScriptAssetWindow ()
		{
			Steup ();

			var window = EditorWindow.GetWindow<ScriptAssetWindow> (false, "MonoScript", true);
			window.ShowPopup ();
		}

		static void Steup ()
		{
			// Get all classes derived from ScriptAssetInstallerBase
			monoScriptTypes = AssemblyHelper.CSharpEditor.GetTypes ().Where (t => t.IsSubclassOf (typeof(ScriptAssetInstallerBase)) && !t.IsGenericType).ToArray ();
			monoScriptDictionary = monoScriptTypes.Select (t => t.Namespace).Distinct ().ToDictionary (k => k, v => monoScriptTypes.Where (t => t.Namespace == v).ToArray ());
		}

		void OnEnable ()
		{
			Steup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("Select a Template Script to Create");
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
				var key0 = monoScriptTypes.Where (t => t.FullName.Length >= searchCondition.Length && t.FullName.StartsWith (searchCondition, true, null)).Select (v => v.Namespace);
				var key1 = monoScriptTypes.Where (t => t.Namespace.Length >= searchCondition.Length && t.Namespace.StartsWith (searchCondition, true, null)).Select (v => v.Namespace);
				var key2 = monoScriptTypes.Where (t => t.Name.Length >= searchCondition.Length && t.Name.StartsWith (searchCondition, true, null)).Select (v => v.Namespace);
				keys = key0.Union (key1).Union (key2).Distinct ().ToArray ();
			} else {
				keys = monoScriptTypes.Select (t => t.Namespace).Distinct ().ToArray ();
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
				var types = monoScriptDictionary [currentKey];
				var name0 = types.Where (t => t.FullName.Length >= searchCondition.Length && t.FullName.StartsWith (searchCondition, true, null)).Select (v => v.Name);
				var name1 = types.Where (t => t.Name.Length >= searchCondition.Length && t.Name.StartsWith (searchCondition, true, null)).Select (v => v.Name);
				names = name0.Union (name1).ToArray ();
			} else {
				names = monoScriptDictionary [currentKey].Select (t => t.Name).ToArray ();
			}
			// If can not search any scriptableobject Name
			if (names == null || names.Length <= 0) {
				return;
			}

			selectedTypeIndex = Mathf.Clamp (selectedTypeIndex, 0, names.Length - 1);
			selectedTypeIndex = EditorGUILayout.Popup (selectedTypeIndex, names);

			var type = monoScriptDictionary [currentKey].Where (t => t.Name == names [selectedTypeIndex]).FirstOrDefault ();
			var isDirty = currentSelectedType == type ? false : true;

			if (isDirty) {
				var guids = AssetDatabase.FindAssets ("t:Object");
				var objs = guids.Select (guid => AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript> (AssetDatabase.GUIDToAssetPath (guid))).Where (o => o != null);
				currentSelectedObject = objs.Where (o => o != null && o.GetClass () == type).Select (o => o as UnityEngine.Object).FirstOrDefault ();
				currentSelectedType = type;
			}

			IScriptAssetInstaller installer = null;
			if (!installers.TryGetValue (type, out installer)) {
				installer = (IScriptAssetInstaller)Activator.CreateInstance (type);
				installers.Add (type, installer);
			}
			if (installer == null) {
				return;
			}

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			EditorGUILayout.ObjectField (currentSelectedObject, typeof(UnityEditor.MonoScript), false);
			EditorGUILayout.TextArea (installer.GetContents ());
			EditorGUILayout.EndScrollView ();

			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Create")) {
				installer.Create ();
			}
		}
	}
}

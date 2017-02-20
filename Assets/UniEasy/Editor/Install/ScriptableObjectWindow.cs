using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniEasy.Edit
{
	/// <summary>
	/// Scriptable object window.
	/// </summary>
	public class ScriptableObjectWindow : EditorWindow
	{
		private int selectedIndex;
		static private string[] scriptableObjectNames;
		static private EndNameEdit endNameEdit;
		static private Type[] types;

		static private Type[] Types { 
			get { return types; }
			set { types = value; }
		}

		public static void Init (Type[] scriptableObjectTypes)
		{
			Types = scriptableObjectTypes;
			scriptableObjectNames = scriptableObjectTypes.Select (o => o.Name).ToArray ();
			endNameEdit = ScriptableObject.CreateInstance<EndNameEdit> ();
			endNameEdit.EndAction += (instanceID, pathName, resourceFile) => {
				AssetDatabase.CreateAsset (EditorUtility.InstanceIDToObject (instanceID), AssetDatabase.GenerateUniqueAssetPath (pathName));
			};
			var window = EditorWindow.GetWindow<ScriptableObjectWindow> (true, "Create a new ScriptableObject", true);
			window.ShowPopup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("ScriptableObject Class");
			selectedIndex = EditorGUILayout.Popup (selectedIndex, scriptableObjectNames);

			if (GUILayout.Button ("Create")) {
				var go = ScriptableObject.CreateInstance (Types [selectedIndex]);
				StartNameEditor.Create (go.GetInstanceID (), endNameEdit,
					string.Format ("{0}.asset", scriptableObjectNames [selectedIndex]),
					AssetPreview.GetMiniTypeThumbnail (typeof(ScriptableObject)), "");

				Close ();
			}
		}
	}
}

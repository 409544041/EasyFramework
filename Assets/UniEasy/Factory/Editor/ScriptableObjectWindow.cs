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
		static private Type[] types;

		static private Type[] Types { 
			get { return types; }
			set { types = value; }
		}

		public static void Init (Type[] scriptableObjectTypes)
		{
			Types = scriptableObjectTypes;
			scriptableObjectNames = scriptableObjectTypes.Select (o => o.Name).ToArray ();
			var window = EditorWindow.GetWindow<ScriptableObjectWindow> (true, "Create a new ScriptableObject", true);
			window.ShowPopup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("ScriptableObject Class");
			selectedIndex = EditorGUILayout.Popup (selectedIndex, scriptableObjectNames);

			if (GUILayout.Button ("Create")) {
				var go = ScriptableObject.CreateInstance (Types [selectedIndex]);
				StartNameEditor.Rename (go.GetInstanceID (), 
					ScriptableObject.CreateInstance<EndNameEdit> (),
					string.Format ("{0}.asset", scriptableObjectNames [selectedIndex]),
					AssetPreview.GetMiniThumbnail (go),	
					"");

				Close ();
			}
		}
	}
}

using UnityEngine;
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
		private int selectedIndex;
		static private string[] scriptableObjectNames;
		static private Type[] types;

		static private Type[] Types { 
			get { return types; }
			set { types = value; }
		}

		[MenuItem ("Assets/Create/UniEasy/ScriptableObject Installer", false, 30)]
		public static void OpenScriptableObjectWindow ()
		{
			var assembly = AssemblyHelper.GetAssemblyCSharp ();

			// Get all classes derived from ScriptableObject
			Types = (from t in assembly.GetTypes ()
			         where t.IsSubclassOf (typeof(ScriptableObject))
			         where !t.IsGenericType
			         select t).ToArray ();
			
			scriptableObjectNames = Types.Select (o => o.Name).ToArray ();

			var window = EditorWindow.GetWindow<ScriptableObjectWindow> (true, "Create a new ScriptableObject", true);
			window.ShowPopup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("ScriptableObject Class");
			selectedIndex = EditorGUILayout.Popup (selectedIndex, scriptableObjectNames);

			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Create")) {
				ScriptableObjectInstaller.CreateScriptableObjectAsset (
					scriptableObjectNames [selectedIndex],
					types [selectedIndex]);
				
				Close ();
			}
		}
	}
}

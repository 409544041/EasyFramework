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
		static private Type[] types = new Type[0];
		static private string[] scriptableObjects = new string[0];

		static private Type[] Types { 
			get { return types; }
			set { types = value; }
		}

		[MenuItem ("Assets/Create/UniEasy/ScriptableObject Installer", false, 30)]
		public static void OpenScriptableObjectWindow ()
		{
			Steup ();

			var window = EditorWindow.GetWindow<ScriptableObjectWindow> (true, "Create a new ScriptableObject", true);
			window.ShowPopup ();
		}

		static void Steup ()
		{
			var assembly = AssemblyHelper.GetAssemblyCSharp ();

			// Get all classes derived from ScriptableObject
			Types = (from t in assembly.GetTypes ()
			         where t.IsSubclassOf (typeof(ScriptableObject))
			         where !t.IsGenericType
			         select t).ToArray ();

			scriptableObjects = Types.Select (o => o.Name).ToArray ();
		}

		void OnEnable ()
		{
			Steup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("ScriptableObject Class");
			selectedIndex = EditorGUILayout.Popup (selectedIndex, scriptableObjects);

			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Create")) {
				ScriptableObjectInstaller.Create (
					scriptableObjects [selectedIndex],
					Types [selectedIndex]);
			}
		}
	}
}

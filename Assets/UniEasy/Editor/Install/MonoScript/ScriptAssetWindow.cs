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
		private int selectedIndex = 0;
		private Vector2 scrollPos = Vector2.zero;
		static private Type[] types = new Type[0];
		static private string[] templates = new string[0];
		static private Dictionary<Type, IScriptAssetInstaller> installers = 
			new Dictionary<Type, IScriptAssetInstaller> ();

		static private Type[] Types { 
			get { return types; }
			set { types = value; }
		}

		[MenuItem ("Assets/Create/UniEasy/Template Script Window", false, 61)]
		static public void OpenScriptAssetWindow ()
		{
			Steup ();

			var window = EditorWindow.GetWindow<ScriptAssetWindow> (true, "Create a Template Script", true);
			window.ShowPopup ();
		}

		static void Steup ()
		{
			var assembly = AssemblyHelper.GetAssemblyCSharpEditor ();

			// Get all classes derived from ScriptAssetInstallerBase
			Types = (from t in assembly.GetTypes ()
			         where t.IsSubclassOf (typeof(ScriptAssetInstallerBase))
			         where !t.IsGenericType
			         select t).ToArray ();

			templates = Types.Select (o => o.Name).ToArray ();
		}

		void OnEnable ()
		{
			Steup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("Select Template");

			selectedIndex = EditorGUILayout.Popup (selectedIndex, templates);

			IScriptAssetInstaller installer;
			if (!installers.TryGetValue (Types [selectedIndex], out installer))
				installer = (IScriptAssetInstaller)Activator.CreateInstance (Types [selectedIndex]);

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			EditorGUILayout.TextArea (installer.GetName () + "\n\n");
			EditorGUILayout.TextArea (installer.GetContents ());
			EditorGUILayout.EndScrollView ();

			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Create")) {
				installer.Create ();
			}
		}
	}
}

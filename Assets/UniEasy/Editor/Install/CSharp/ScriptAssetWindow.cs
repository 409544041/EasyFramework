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
		private int selectedIndex;
		private Vector2 scrollPos;
		private Dictionary<Type, IScriptAssetInstaller> installers = new Dictionary<Type, IScriptAssetInstaller> ();
		static private string[] scriptAssetNames;
		static private Type[] types;

		static private Type[] Types { 
			get { return types; }
			set { types = value; }
		}

		[MenuItem ("Assets/Create/UniEasy/Template CSharp Installer", false, 20)]
		public static void OpenScriptAssetWindow ()
		{
			var assembly = EasyAssembly.GetAssemblyCSharpEditor ();

			// Get all classes derived from ScriptAssetInstallerBase
			Types = (from t in assembly.GetTypes ()
			         where t.IsSubclassOf (typeof(ScriptAssetInstallerBase))
			         where !t.IsGenericType
			         select t).ToArray ();

			scriptAssetNames = Types.Select (o => o.Name).ToArray ();

			var window = EditorWindow.GetWindow<ScriptAssetWindow> (true, "Create a Template Script", true);
			window.ShowPopup ();
		}

		public void OnGUI ()
		{
			GUILayout.Label ("Select Template");
			selectedIndex = EditorGUILayout.Popup (selectedIndex, scriptAssetNames);

			IScriptAssetInstaller installer;
			if (!installers.TryGetValue (types [selectedIndex], out installer))
				installer = (IScriptAssetInstaller)Activator.CreateInstance (types [selectedIndex]);

			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			EditorGUILayout.TextArea (installer.GetScriptAssetName () + ".cs\n\n");
			EditorGUILayout.TextArea (installer.GetScriptAssetContents ());
			EditorGUILayout.EndScrollView ();

			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Create")) {
				ScriptAssetUlit.CreateScriptAsset (
					installer.GetScriptAssetName (),
					installer.GetScriptAssetContents ());

				Close ();
			}
		}
	}
}

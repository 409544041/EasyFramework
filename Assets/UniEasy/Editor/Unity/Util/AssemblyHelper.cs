using UnityEditor;
using System.Reflection;
using System;

namespace UniEasy.Edit
{
	public class AssemblyHelper
	{
		private static Assembly cSharpEditor;
		private static Assembly editorWindow;
		private static Assembly sceneView;
		private static Assembly cSharp;

		public static Assembly CSharp {
			get {
				if (cSharp == null)
					cSharp = Assembly.Load (new AssemblyName ("Assembly-CSharp"));
				return cSharp;
			}
		}

		public static Assembly CSharpEditor {
			get {
				if (cSharpEditor == null)
					cSharpEditor = Assembly.Load (new AssemblyName ("Assembly-CSharp-Editor"));
				return cSharpEditor;
			}
		}

		public static Assembly EditorWindow {
			get {
				if (editorWindow == null)
					editorWindow = Assembly.GetAssembly (typeof(EditorWindow));
				return editorWindow;
			}
		}

		public static Assembly SceneView {
			get {
				if (sceneView == null)
					sceneView = Assembly.GetAssembly (typeof(UnityEditor.SceneView));
				return sceneView;
			}
		}
	}
}

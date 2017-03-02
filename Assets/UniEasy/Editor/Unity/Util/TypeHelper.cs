using System.Reflection;
using System;

namespace UniEasy.Edit
{
	public class TypeHelper
	{
		private static Type internalEditorUtilityType;
		private static Type sceneHierarchyWindow;
		private static Type consoleWindowType;
		private static Type logEntriesType;
		private static Type logEntryType;

		public static Type GetType (string filePath, string typeName)
		{
			Assembly assembly = Assembly.LoadFile (filePath);
			return assembly.GetType (typeName);
		}

		public static Type GetType (AssemblyName assemblyName, string typeName)
		{
			Assembly assembly = Assembly.Load (assemblyName);
			return assembly.GetType (typeName);
		}

		static public Type SceneHierarchyWindow {
			get {
				if (sceneHierarchyWindow == null) {
//		    		return GetType ("c:/program files/unity/editor/data/managed/UnityEditor.dll", "UnityEditor.SceneHierarchyWindow");
					AssemblyName assemblyName = new AssemblyName ("UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
					sceneHierarchyWindow = GetType (assemblyName, "UnityEditor.SceneHierarchyWindow");
				}
				return sceneHierarchyWindow;
			}
		}

		public static Type InternalEditorUtilityType {
			get {
				if (internalEditorUtilityType == null)
					internalEditorUtilityType = AssemblyHelper.SceneView.GetType ("UnityEditorInternal.InternalEditorUtility");
				return internalEditorUtilityType;
			}
		}

		public static Type ConsoleWindowType {
			get {
				if (consoleWindowType == null)
					consoleWindowType = AssemblyHelper.EditorWindow.GetType ("UnityEditor.ConsoleWindow");
				return consoleWindowType;
			}
		}

		public static Type LogEntriesType {
			get {
				if (logEntriesType == null)
					logEntriesType = AssemblyHelper.EditorWindow.GetType ("UnityEditorInternal.LogEntries");
				return logEntriesType;
			}
		}

		public static Type LogEntryType {
			get {
				if (logEntryType == null)
					logEntryType = AssemblyHelper.EditorWindow.GetType ("UnityEditorInternal.LogEntry");
				return logEntryType;
			}
		}
	}
}

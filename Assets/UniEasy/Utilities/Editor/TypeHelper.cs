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
				if (internalEditorUtilityType == null) {
					internalEditorUtilityType = AssemblyHelper.SceneView.GetType ("UnityEditorInternal.InternalEditorUtility");
				}
				return internalEditorUtilityType;
			}
		}

		public static Type ConsoleWindowType {
			get {
				if (consoleWindowType == null) {
					consoleWindowType = AssemblyHelper.EditorWindow.GetType ("UnityEditor.ConsoleWindow");
				}
				return consoleWindowType;
			}
		}

		public static Type LogEntriesType {
			get {
				if (logEntriesType == null) {
					#if UNITY_2017_1_OR_NEWER
					logEntriesType = AssemblyHelper.EditorWindow.GetType ("UnityEditor.LogEntries");
					#else
					logEntriesType = AssemblyHelper.EditorWindow.GetType ("UnityEditorInternal.LogEntries");
					#endif
				}
				return logEntriesType;
			}
		}

		public static Type LogEntryType {
			get {
				if (logEntryType == null) {
					#if UNITY_2017_1_OR_NEWER
					logEntryType = AssemblyHelper.EditorWindow.GetType ("UnityEditor.LogEntry");
					#else
					logEntryType = AssemblyHelper.EditorWindow.GetType ("UnityEditorInternal.LogEntry");
					#endif
				}
				return logEntryType;
			}
		}
	}
}

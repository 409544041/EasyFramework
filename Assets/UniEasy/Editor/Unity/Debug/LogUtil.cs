using UnityEditor;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System;

namespace UniEasy.Edit
{
	public class LogUtil
	{
		private static string projectPath;
		private static MethodInfo logEntriesGetEntry;
		private static FieldInfo logListViewCurrentRow;
		private static FieldInfo logEntryCondition;
		private static object logListView;
		private static object consoleWindow;
		private static object logEntry;

		public static string ProjectPath {
			get {
				if (string.IsNullOrEmpty (projectPath))
					projectPath = System.IO.Path.GetDirectoryName (UnityEngine.Application.dataPath);
				return projectPath;
			}
		}

		[UnityEditor.Callbacks.OnOpenAssetAttribute (0)]  
		public static bool OnOpenAsset (int instanceID, int line)
		{
			int lineNumber = 1;
			string filename = "";
			if (GetDoubleClickLine (out filename, out lineNumber)) {
				if (lineNumber == -1) {
					OpenFileAtLineExternal (ProjectPath + "/" + filename, 1);
				} else {
					OpenFileAtLineExternal (ProjectPath + "/" + filename, lineNumber); 
				}
				return true;
			}
			// didn't find a code file? let unity figure it out
			return false;
		}

		private static void GetConsoleWindowListView ()
		{  
			if (logListView == null) {
				var fieldInfo = TypeHelper.ConsoleWindowType.GetField ("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
				consoleWindow = fieldInfo.GetValue (null);
				var listViewFieldInfo = TypeHelper.ConsoleWindowType.GetField ("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
				logListView = listViewFieldInfo.GetValue (consoleWindow);
				logListViewCurrentRow = listViewFieldInfo.FieldType.GetField ("row", BindingFlags.Instance | BindingFlags.Public); 
				logEntriesGetEntry = TypeHelper.LogEntriesType.GetMethod ("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
				logEntry = Activator.CreateInstance (TypeHelper.LogEntryType);
				logEntryCondition = TypeHelper.LogEntryType.GetField ("condition", BindingFlags.Instance | BindingFlags.Public); 
			}
		}

		private static bool GetDoubleClickLine (out string filename, out int line)
		{  
			line = -1;
			filename = "";
			GetConsoleWindowListView ();
			if (logListView == null)
				return false;
			else {  
				int row = (int)logListViewCurrentRow.GetValue (logListView);
				if (row < 0)
					return false;
				logEntriesGetEntry.Invoke (null, new object[] {
					row,
					logEntry
				});
				var condition = logEntryCondition.GetValue (logEntry).ToString ();
				var lines = condition.Split (new char[] {
					'\n',
				});
				var helpful = lines.Where (l => !l.Contains ("UnityEngine.Debug") &&
				              !l.Contains ("UnityEngine.Logger") &&
				              !l.Contains ("UniEasy.Debugger") &&
				              !string.IsNullOrEmpty (l)).ToArray ();
				if (helpful.Length < 2)
					return false;
				var content = helpful.GetValue (1).ToString ();
				var startIndex = content.IndexOf ("(at ") + 4;
				var endIndex = content.IndexOf (")", startIndex);
				if (startIndex == -1 || endIndex == -1)
					return false;
				var target = content.Substring (startIndex, endIndex - startIndex).Split (new char[] {
					':',
				});
				filename = target [0];
				int.TryParse (target [1], out line);
				return true;  
			}
		}

		private static void OpenFileAtLineExternal (string filename, int line)
		{
			var method = TypeHelper.InternalEditorUtilityType.GetMethod ("OpenFileAtLineExternal");
			method.Invoke (method, new object[] {
				filename,
				line
			});
		}
	}
}
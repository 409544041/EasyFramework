using UnityEngine;
using System.Linq;
using System;

namespace UniEasy
{
	public class Debugger
	{
		private static DebuggerMask debuggerMask;
		private static bool showOnUGUI;

		public static event Action<string> BeforeCheckLayerInEditorEvent;

		public static bool IsLogEnabled {
			get {
				return Debug.logger.logEnabled;
			}
			set {
				Debug.logger.logEnabled = value;
			}
		}

		public static bool ShowOnUGUI {
			get {
				return showOnUGUI;
			}
			set {
				showOnUGUI = value;
			}
		}

		public static bool IsLogLayerAllowed (string layerName)
		{
			if (IsLogEnabled && GetLayerMask ().Contains (layerName))
				return true;
			return false;
		}

		public static string[] GetLayerMask ()
		{
			if (debuggerMask == null) {
				debuggerMask = new DebuggerMask ("Default");
			}
			return debuggerMask.GetLayerNames ();
		}

		public static void SetLayerMask (params string[] layerNames)
		{
			debuggerMask = new DebuggerMask (layerNames);
		}

		public static void AddLayerMask (params string[] layerNames)
		{
			var second = GetLayerMask ();
			debuggerMask.SetLayerNames (layerNames.Union (second).ToArray ());
		}

		public static void RemoveLayerMask (params string[] layerNames)
		{
			var origin = GetLayerMask ();
			var second = origin.Intersect (layerNames);
			var values = origin.Except (second).ToArray ();
			debuggerMask.SetLayerNames (values);
		}

		private static void Log (LogType logType, object message, string layerName, UnityEngine.Object context)
		{
			#if UNITY_EDITOR
			if (BeforeCheckLayerInEditorEvent != null) {
				BeforeCheckLayerInEditorEvent.Invoke (layerName);
			}
			#endif
			if (IsLogLayerAllowed (layerName)) {
				Debug.logger.Log (logType, message, context);
			}
			if (ShowOnUGUI) {
				
			}
		}

		public static void Log (object message, string layerName = "Default", UnityEngine.Object context = null)
		{
			Log (LogType.Log, message, layerName, context);
		}

		public static void LogWarnning (object message, string layerName = "Default", UnityEngine.Object context = null)
		{
			Log (LogType.Warning, message, layerName, context);
		}

		public static void LogError (object message, string layerName = "Default", UnityEngine.Object context = null)
		{
			Log (LogType.Error, message, layerName, context);
		}
	}
}

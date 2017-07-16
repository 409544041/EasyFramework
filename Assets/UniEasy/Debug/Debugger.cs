using UnityEngine;
using System.Linq;
using System;

namespace UniEasy.Console
{
	public class Debugger
	{
		private static DebugMask debugMask;
		private static bool logEnabled;
		private static bool showOnUGUI;

		public static event Action<string> BeforeCheckLayerInEditorEvent;
		public static event Action<LogType, object> OnLogEvent;

		public static bool IsLogEnabled {
			get {
				return logEnabled;
			}
			set {
				logEnabled = value;
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
			if (IsLogEnabled && GetLayerMask ().Contains (layerName)) {
				return true;
			}
			return false;
		}

		public static string[] GetLayerMask ()
		{
			if (debugMask == null) {
				debugMask = new DebugMask ("Default");
			}
			return debugMask.ToList ().Select (x => x.layerName).ToArray ();
		}

		public static void SetLayerMask (params string[] layerNames)
		{
			debugMask = new DebugMask (layerNames);
		}

		public static void AddLayerMask (params string[] layerNames)
		{
			var second = GetLayerMask ();
			debugMask = new DebugMask (layerNames.Union (second).ToArray ());
		}

		public static void RemoveLayerMask (params string[] layerNames)
		{
			var origin = GetLayerMask ();
			var second = origin.Intersect (layerNames);
			var values = origin.Except (second).ToArray ();
			debugMask = new DebugMask (values);
		}

		private static void Log (LogType logType, object message, string layerName, UnityEngine.Object context)
		{
			#if UNITY_EDITOR
			if (BeforeCheckLayerInEditorEvent != null) {
				BeforeCheckLayerInEditorEvent.Invoke (layerName);
			}
			#endif
			if (IsLogLayerAllowed (layerName)) {
				Debug.unityLogger.Log (logType, message, context);
			}
			if (ShowOnUGUI && OnLogEvent != null) {
				OnLogEvent.Invoke (logType, message);
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

using UnityEngine;

namespace UniEasy.Console
{
	public class Console
	{
		public static void Log (string line)
		{
			#if UNITY_EDITOR
			Debug.Log (line);
			#endif
		}

		public static string ExecuteCommand (string command, params string[] args)
		{
			return CommandLibrary.ExecuteCommand (command, args);
		}
	}
}

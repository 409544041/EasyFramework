using UnityEngine;

namespace UniEasy.Console
{
	public static class QuitCommand
	{
		public static readonly string name = "Quit";
		public static readonly string description = "Quit the application.";
		public static readonly string usage = "Quit";

		public static string Execute (params string[] args)
		{
			Application.Quit ();
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#endif
			return "Quitting...";
		}
	}
}

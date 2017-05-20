using UnityEngine.SceneManagement;
using System.IO;

namespace UniEasy.Console
{
	public class LoadSceneCommand
	{
		public static readonly string name = "LoadScene";
		public static readonly string description = "Load the specified scene by name. Before you can load a scene you have to add it to the list of levels used in the game. Use File->Build Settings... in Unity and add the levels you need to the level list there.";
		public static readonly string usage = "LoadScene sceneName";

		public static string Execute (params string[] args)
		{
			if (args.Length == 0) {
				return HelpCommand.Execute (LoadSceneCommand.name);
			} else {
				return LoadScene (args [0]);
			}
		}

		private static string LoadScene (string sceneName)
		{
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
				var scene = Path.GetFileNameWithoutExtension (SceneUtility.GetScenePathByBuildIndex (i));
				if (scene.ToLower () == sceneName.ToLower ()) {
					SceneManager.LoadScene (sceneName);
					return string.Format ("Success loading {0}.", sceneName);
				}
			}
			return string.Format ("Failed to load {0} scene. Are you sure it's in the list of levels in Build Settings?", sceneName);
		}
	}
}

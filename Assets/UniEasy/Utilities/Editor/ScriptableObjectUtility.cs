using UnityEditor;
using UnityEngine;
using System.IO;

namespace UniEasy
{
	public class ScriptableObjectUtility
	{
		static public T LoadAtPath<T> (string assetPath) where T : ScriptableObject
		{
			return (T)AssetDatabase.LoadAssetAtPath<ScriptableObject> (assetPath);
		}

		static public T CreateAsset<T> (T asset, string savePath) where T : ScriptableObject
		{
			var directoryName = Path.GetDirectoryName (savePath);
			if (!Directory.Exists (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			// You can not create assets(eg. asset) directly in the streamingassets folder,
			// The assets be created have some issue can not be read,
			// So We need to create assets outside first,
			// Then move them to the streamingassets folder.
			if (savePath.Contains (Application.streamingAssetsPath)) {
				var fileName = Path.GetFileName (savePath);
				var tempPath = string.Format ("Assets/{0}", fileName);
				var result = DirectCreateAsset<T> (asset, tempPath);
				var newPath = savePath.Substring (savePath.IndexOf ("Assets"));
				AssetDatabase.DeleteAsset (newPath);
				AssetDatabase.MoveAsset (tempPath, newPath);
				AssetDatabase.Refresh ();
				return result;
			}
			return DirectCreateAsset<T> (asset, savePath);
		}

		static public T DirectCreateAsset<T> (T asset, string savePath) where T : ScriptableObject
		{
			if (File.Exists (savePath)) {
				var o = LoadAtPath<ScriptableObject> (savePath);
				var json = JsonUtility.ToJson (asset);
				JsonUtility.FromJsonOverwrite (json, o);
			} else {
				AssetDatabase.CreateAsset (asset as Object, AssetDatabase.GenerateUniqueAssetPath (savePath));
			}
			AssetDatabase.Refresh ();
			return LoadAtPath<T> (savePath);
		}

		static public void SaveAssetPanel<T> (T asset, string defaultName = "New ScriptableObject") where T : ScriptableObject
		{
			var path = EditorUtility.SaveFilePanel ("Save 'ScriptableObject' file to folder", Application.dataPath, defaultName, "asset");
			if (!string.IsNullOrEmpty (path)) {
				if (path.Contains (Application.dataPath)) {
					var savePath = path.Replace (Application.dataPath, "Assets");
					CreateAsset (asset, savePath);
					ProjectWindowUtil.ShowCreatedAsset (AssetDatabase.LoadAssetAtPath<ScriptableObject> (savePath));
				} else {
					Debug.LogError ("Sorry, we cannot save 'ScriptableObject' file out of 'Assets' folder!");
				}
			}
		}
	}
}

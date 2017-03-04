﻿using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace UniEasy
{
	public class ScriptableObjectFactory
	{
		static public void SaveAssetPanel<T> (T asset, string defaultName = "New ScriptableObject")
		{
			#if UNITY_EDITOR
			string path = EditorUtility.SaveFilePanel ("Save .asset file to folder", Application.dataPath, defaultName, "asset");
			if (!string.IsNullOrEmpty (path)) {
				if (path.Contains (Application.dataPath)) {
					string assetPath = path.Replace (Application.dataPath, "Assets");
					CreateAsset (asset, assetPath);
					ProjectWindowUtil.ShowCreatedAsset (AssetDatabase.LoadAssetAtPath<Object> (assetPath));
				} else
					Debug.LogError ("Sorry, we can't save .asset file out of assets folder!");
			}
			#endif
		}

		static public T CreateAsset<T> (T asset, string assetPath)
		{
			var directoryName = Path.GetDirectoryName (assetPath);
			if (!Directory.Exists (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			#if UNITY_EDITOR
			// If you create directly in the streamingassets folder,
			// The file created is not correct.
			if (assetPath.Contains (Application.streamingAssetsPath)) {
				var fileName = Path.GetFileName (assetPath);
				var tempPath = "Assets/" + fileName;
				var result = DirectCreateAsset<T> (asset, tempPath);
				var newPath = assetPath.Replace (Application.streamingAssetsPath, "Assets/StreamingAssets");
				AssetDatabase.DeleteAsset (newPath);
				AssetDatabase.MoveAsset (tempPath, newPath);
				AssetDatabase.Refresh ();
				return result;
			}
			return DirectCreateAsset<T> (asset, assetPath);
			#else
			return default (T);
			#endif
		}

		private static T DirectCreateAsset<T> (T asset, string assetPath)
		{
			#if UNITY_EDITOR
			if (asset.GetType ().IsSubclassOf (typeof(ScriptableObject))) {
				if (File.Exists (assetPath)) {
					Object o = LoadAssetAtPath<Object> (assetPath);
					string json = JsonUtility.ToJson (asset);
					JsonUtility.FromJsonOverwrite (json, o);
				} else
					AssetDatabase.CreateAsset (asset as Object, AssetDatabase.GenerateUniqueAssetPath (assetPath));
				AssetDatabase.Refresh ();
				return LoadAssetAtPath<T> (assetPath);
			} else {
				Debug.LogError ("Can not create asset file, the type is not Sub class of ScriptableObject!");
				return default (T);
			}
			#else
			return default (T);
			#endif
		}

		static public T LoadAssetAtPath<T> (string assetPath)
		{
			#if UNITY_EDITOR
			Object o = AssetDatabase.LoadAssetAtPath<Object> (assetPath);
			(o as IScriptableObjectLoadCallback).OnAfterAssetLoaded ();
			return (T)((object)o);
			#else
			return default (T);
			#endif
		}
	}
}

using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

public interface IScriptableObjectLoadCallback
{
	void OnAfterAssetLoaded ();
}

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
		#if UNITY_EDITOR
		if (asset.GetType ().IsSubclassOf (typeof(ScriptableObject))) {
			if (File.Exists (assetPath)) {
				AssetDatabase.CreateAsset (asset as Object, assetPath);
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

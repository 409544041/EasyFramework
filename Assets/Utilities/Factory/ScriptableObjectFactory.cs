using UnityEngine;

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
	static public T CreateAsset<T> (T asset, string assetPath)
	{
		#if UNITY_EDITOR
		if (asset.GetType ().IsSubclassOf (typeof(ScriptableObject))) {
			AssetDatabase.DeleteAsset (assetPath);
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

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class EasyAsset : EasyAsset<Object>
{
	protected override string GetKey (Object item)
	{
		return (item as Object).name;
	}
}

[System.Serializable]
public class EasyAsset<T> : ScriptableObject,IScriptableObjectLoadCallback
{
	[SerializeField]
	private List<T> values;
	private Dictionary<string, T> target;

	public int Count {
		get {
			return values.Count;
		}
	}

	public Dictionary<string, T> ToDictionary ()
	{
		return target;
	}

	protected virtual string GetKey (T item)
	{
		return null;
	}

	public void OnAfterAssetLoaded ()
	{
		this.OnEnable ();
	}

	public void OnEnable ()
	{
		if (values == null)
			values = new List<T> ();
		target = new Dictionary<string, T> ();
		if (values != null && values.Count > 0)
			AddRange (values.ToArray ());
	}

	void OnDestroy ()
	{
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void Add (T item)
	{
		if (item == null)
			return;
		if (!values.Contains (item)) {
			values.Add (item);
		}
		if (!target.ContainsKey (GetKey (item))) {
			target.Add (GetKey (item), item);
		}
	}

	public void AddRange (T[] items)
	{
		for (int i = 0; i < items.Length; i++) {
			Add (items [i]);
		}
	}

	public void Remove (T item)
	{
		if (item == null)
			return;
		if (values.Contains (item))
			values.Remove (item);
		if (target.ContainsKey (GetKey (item)))
			target.Remove (GetKey (item));
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void Remove (string name)
	{
		if (string.IsNullOrEmpty (name))
			return;
		if (target.ContainsKey (name)) {
			T o = target [name];
			if (values.Contains (o)) {
				values.Remove (o);
			}
			target.Remove (name);
		}
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void RemoveRange (T[] items)
	{
		for (int i = 0; i < items.Length; i++) {
			Remove (items [i]);
		}
	}

	public void RemoveRange (string[] names)
	{
		for (int i = 0; i < names.Length; i++) {
			Remove (names [i]);
		}
	}

	public void Clear ()
	{
		target = new Dictionary<string, T> ();
		values = new List<T> ();
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}
}
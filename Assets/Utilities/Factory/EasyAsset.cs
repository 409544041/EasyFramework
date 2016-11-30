using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class EasyAsset : ScriptableObject,IScriptableObjectLoadCallback
{
	[SerializeField]
	private List<Object> values;
	private Dictionary<string, Object> target;

	public Dictionary<string, Object> ToDictionary ()
	{
		return target;
	}

	public void OnAfterAssetLoaded ()
	{
		this.Awake ();
	}

	public void Awake ()
	{
		if (values == null)
			values = new List<Object> ();
		target = new Dictionary<string, Object> ();
		if (values != null && values.Count > 0)
			AddRange (values.ToArray ());
	}

	void OnDestroy ()
	{
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void Add (Object item)
	{
		if (item == null)
			return;
		if (!values.Contains (item)) {
			values.Add (item);
		}
		if (!target.ContainsKey (item.name)) {
			target.Add (item.name, item);
		}
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void AddRange (Object[] items)
	{
		for (int i = 0; i < items.Length; i++) {
			Add (items [i]);
		}
	}

	public void Remove (Object item)
	{
		if (item == null)
			return;
		if (values.Contains (item))
			values.Remove (item);
		if (target.ContainsKey (item.name))
			target.Remove (item.name);
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void Remove (string name)
	{
		if (string.IsNullOrEmpty (name))
			return;
		if (target.ContainsKey (name)) {
			Object o = target [name];
			if (values.Contains (o)) {
				values.Remove (o);
			}
			target.Remove (name);
		}
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}

	public void RemoveRange (Object[] items)
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
		target = new Dictionary<string, Object> ();
		values = new List<Object> ();
		#if UNITY_EDITOR
		AssetDatabase.SaveAssets ();
		#endif
	}
}
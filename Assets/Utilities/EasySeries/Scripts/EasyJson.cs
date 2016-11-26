using UnityEngine;
using System.Collections.Generic;
using System;

// create by chaolun 2016/11/26
[Serializable]
public class EasyJson<T>
{
	[SerializeField]
	private List<T> target;

	public List<T> ToList ()
	{
		return target;
	}

	public EasyJson (List<T> target)
	{
		this.target = target;
	}
}

[Serializable]
public class EasyJson<TKey, TValue> : ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys;
	[SerializeField]
	private List<TValue> values;
	private Dictionary<TKey, TValue> target;

	public EasyJson (Dictionary<TKey, TValue> target)
	{
		this.target = target;
	}

	public void OnBeforeSerialize ()
	{
		keys = new List<TKey> (target.Keys);
		values = new List<TValue> (target.Values);
	}

	public void OnAfterDeserialize ()
	{
		var count = Math.Min (keys.Count, values.Count);
		target = new Dictionary<TKey, TValue> (count);
		for (var i = 0; i < count; ++i) {
			target.Add (keys [i], values [i]);
		}
	}

	public Dictionary<TKey, TValue> ToDictionary ()
	{
		return target;
	}
}
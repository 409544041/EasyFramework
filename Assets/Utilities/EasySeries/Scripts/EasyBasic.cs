using UnityEngine;
using System.Collections.Generic;
using System;

// create by chaolun 2016/11/26
[Serializable]
public partial class EasyBasic<T>
{
	[SerializeField]
	protected List<T> value;

	public List<T> ToList ()
	{
		return this.value;
	}

	public EasyBasic ()
	{
		
	}

	public EasyBasic (List<T> value)
	{
		this.value = value;
	}

	public EasyBasic (T[] value)
	{
		this.value = new List<T> ();
		this.value.AddRange (value);
	}
}

[Serializable]
public partial class EasyBasic<TKey, TValue> : ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys;
	[SerializeField]
	private List<TValue> values;
	private Dictionary<TKey, TValue> target;

	public EasyBasic (Dictionary<TKey, TValue> target)
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
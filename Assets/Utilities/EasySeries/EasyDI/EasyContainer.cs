using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

public class EasyContainer
{
	static private Dictionary<Type, List<object>> dictionary = new Dictionary<Type, List<object>> ();

	static public EasyContainer<T> CreateInstantiate<T> ()
	{
		object value = default (T);
		Type type = typeof(T);
		if (type.IsSubclassOf (typeof(MonoBehaviour))) {
			var go = new GameObject (type.Name);
			value = go.AddComponent (type);
		} else
			value = Activator.CreateInstance (type);
		if (dictionary.ContainsKey (type)) {
			if (dictionary [type] == null)
				dictionary [type] = new List<object> () { value };
			else if (!dictionary [type].Contains (value))
				dictionary [type].Add (value);
		} else
			dictionary.Add (type, new List<object> () { value });
		var result = new EasyContainer<T> ((T)value);
		MessageBroker.Default.Publish<EasyContainer<T>> (result);
		return result;
	}

	static public EasyContainer<T> Bind<T> ()
	{
		Type type = typeof(T);
		if (dictionary.ContainsKey (type) && dictionary [type].Count > 0) {
			var result = new EasyContainer<T> ((T)dictionary [type] [0]);
			return result;
		}
		return default (EasyContainer<T>);
	}
}

public class EasyContainer<T>
{
	private T value;

	public T Value {
		get {
			return value;
		}
		set {
			this.value = value;
		}
	}

	public EasyContainer (T value)
	{
		this.value = value;
	}
}

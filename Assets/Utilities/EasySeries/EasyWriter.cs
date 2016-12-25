using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

public partial class EasyWriter : IDisposable
{
	private string filePath;
	private static Dictionary<string, EasyData<string, EasyData>> files;

	public EasyData<string, EasyData> target {
		get {
			return files [filePath];
		}
	}

	public EasyWriter (string path)
	{
		filePath = path;
		if (files == null) {
			files = new Dictionary<string, EasyData<string, EasyData>> ();
			Observable.OnceApplicationQuit ().Subscribe (_ => {
				Dispose ();
			});
		}
		if (!files.ContainsKey (filePath)) {
			var value = Deserialize<EasyData<string, EasyData>> (filePath);
			if (value != null)
				files.Add (filePath, value);
			else
				files.Add (filePath, new EasyData<string, EasyData> (new Dictionary<string, EasyData> ()));
		}
	}

	public void Dispose ()
	{
		Serialize<EasyData<string, EasyData>> (filePath, target);
	}

	public object GetObject (string key)
	{
		if (target.ToDictionary ().ContainsKey (key)) {
			return target.ToDictionary () [key].GetObject ();
		}
		return default (object);
	}

	public void SetObject (string key, object value)
	{
		if (target.ToDictionary ().ContainsKey (key)) {
			target.ToDictionary () [key] = new EasyData (value);
		} else {
			target.ToDictionary ().Add (key, new EasyData (value));
		}
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			Dispose ();
		}
		#endif
	}

	public T Get<T> (string key)
	{
		Type type = typeof(T);
		if (type == typeof(string)) {
			return (T)GetObject (key);
		} else if (type.IsArray) {
			Debug.LogError ("Sorry, we can not auto convert string to array type, " +
			"you can used Get<string> () function replaced, " +
			"then call EasyWriter.ConvertStringToArray<T> function convert string to target type.");
		} else if (type.IsSerializable && type.IsPrimitive) {
			return (T)GetObject (key);
		} else if (type.IsSerializable && type.IsEnum) {
		} else if (type.IsSerializable && type == typeof(Nullable)) {
		} else if (type.IsSerializable && (type.IsClass || type.IsValueType)) {
			if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
				Debug.LogError ("Only plain classes and structures are supported; " +
				"classes derived from UnityEngine.Object (such as MonoBehaviour or ScriptableObject) are not." +
				"so please use Get<T> (string key, T target) replaced.");
			} else {
				return JsonUtility.FromJson<T> (GetObject (key).ToString ());
			}
		}
		return default (T);
	}

	public void Get<T> (string key, T target)
	{
		Type type = target.GetType ();
		if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
			JsonUtility.FromJsonOverwrite (GetObject (key).ToString (), target);
		}
	}

	public void GetArray<T> (string key, T[] target)
	{
		string[] array = GetArray<string> (key);
		for (int i = 0; i < array.Length && i < target.Length; i++) {
			Type type = target [i].GetType ();
			if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
				JsonUtility.FromJsonOverwrite (array [i], target [i]);
			}
		}
	}

	public T[] GetArray<T> (string key)
	{
		string content = Get<string> (key);
		if (!string.IsNullOrEmpty (content)) {
			return EasyConvert.StringToArray<T> (content);
		}
		return default (T[]);
	}

	public void Set<T> (string key, T value)
	{
		Type type = typeof(T);
		if (value == null) {
			Debug.LogError ("NullReferenceException: Object reference not set to an instance of an object");
		} else if (type == typeof(string)) {
			SetObject (key, value);
		} else if (type.IsArray) {
			Debug.LogError ("Sorry, we can not auto convert array to string type, " +
			"you can used EasyWriter.ConvertArrayToString () function convert to string first, " +
			"then call this function to save data.");
		} else if (type.IsSerializable && type.IsPrimitive) {
			SetObject (key, value);
		} else if (type.IsSerializable && type.IsEnum) {
		} else if (type.IsSerializable && type == typeof(Nullable)) {
		} else if (type.IsSerializable && (type.IsClass || type.IsValueType)) {
			#if UNITY_EDITOR
			SetObject (key, JsonUtility.ToJson (value, true));
			#else
			SetObject (key, JsonUtility.ToJson (value));
			#endif
		}
	}

	public void SetArray<T> (string key, object value)
	{
		string content = default (string);
		if (!value.GetType ().IsArray) {
			T[] o = new object[] { value } as T[];
			content = EasyConvert.ArrayToString<T> (o);
		} else
			content = EasyConvert.ArrayToString<T> (value);
		Set<string> (key, content);
	}

	public void Remove (string key)
	{
		var dictionary = target.ToDictionary ();
		if (dictionary != null && dictionary.ContainsKey (key)) {
			dictionary.Remove (key);
			files [filePath] = new EasyData<string, EasyData> (dictionary);
		}
	}

	public void Clear ()
	{
		files [filePath] = new EasyData<string, EasyData> (new Dictionary<string, EasyData> ());
	}
}

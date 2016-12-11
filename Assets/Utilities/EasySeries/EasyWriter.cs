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
		if (!files.ContainsKey (filePath))
			files.Add (filePath, Deserialize<EasyData<string, EasyData>> (filePath));
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
		if (type == (typeof(string))) {
			return (T)GetObject (key);
		} else if (type.IsSerializable && type.IsArray) {
			Debug.LogError ("Sorry, we can not auto convert string to array type, " +
			"you can used Get<string> () function replaced, " +
			"then call EasyWriter.ConvertStringToArray<T> function convert string to target type.");
		} else if (type.IsSerializable && (type.IsClass || (type.IsValueType && !type.IsPrimitive))) {
			if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
				Debug.LogError ("Only plain classes and structures are supported; " +
				"classes derived from UnityEngine.Object (such as MonoBehaviour or ScriptableObject) are not." +
				"so please use Get<T> (string key, ref T target) replaced.");
			} else {
				return JsonUtility.FromJson<T> (GetObject (key).ToString ());
			}
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsPrimitive) {
				return (T)GetObject (key);
			}
		}
		return (T)GetObject (key);
	}

	public void Get<T> (string key, ref T target)
	{
		Type type = target.GetType ();
		if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
			JsonUtility.FromJsonOverwrite (GetObject (key).ToString (), target);
		}
	}

	public T[] GetArray<T> (string key)
	{
		string content = Get<string> (key);
		if (!string.IsNullOrEmpty (content)) {
			return ConvertStringToArray<T> (content);
		}
		return default (T[]);
	}

	public void Set<T> (string key, T value)
	{
		Type type = typeof(T);
		if (type == (typeof(string))) {
			SetObject (key, value);
		} else if (type.IsSerializable && type.IsArray) {
			Debug.LogError ("Sorry, we can not auto convert array to string type, " +
			"you can used EasyWriter.ConvertArrayToString () function convert to string first, " +
			"then call this function to save data.");
		} else if (type.IsSerializable && (type.IsClass || (type.IsValueType && !type.IsPrimitive))) {
			#if UNITY_EDITOR
			SetObject (key, JsonUtility.ToJson (value, true));
			#else
			SetObject (key, JsonUtility.ToJson (value));
			#endif
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsPrimitive) {
				SetObject (key, value);
			}
		}
	}

	public void SetArray<T> (string key, object value)
	{
		string content = default (string);
		if (!value.GetType ().IsArray) {
			T[] o = new object[] { value } as T[];
			content = ConvertArrayToString<T> (o);
		} else
			content = ConvertArrayToString<T> (value);
		Set<string> (key, content);
	}

	static public string ConvertArrayToString<T> (object value)
	{
		Type type = typeof(T);
		T[] array = default (T[]);
		if (value.GetType () == typeof(List<T>)) {
			array = (value as List<T>).ToArray ();
		} else if (value.GetType () == typeof(T[])) {
			array = value as T[];
		} else {
			Debug.LogError ("Sorry, this function only support T[] or List<T>!");
			return default (string);
		}
		if (type == (typeof(string))) {
			return JsonUtility.ToJson (new EasyStrings (array as string[]));
		} else if (type.IsSerializable && (type.IsClass || (type.IsValueType && !type.IsPrimitive))) {
			string[] content = new string[array.Length];
			for (int i = 0; i < content.Length; i++) {
				content [i] = JsonUtility.ToJson (array [i]);
			}
			return JsonUtility.ToJson (new EasyStrings (content));
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsPrimitive) {
				EasyData[] datas = new EasyData[array.Length];
				for (int i = 0; i < datas.Length; i++) {
					datas [i] = new EasyData (array [i]);
				}
				return JsonUtility.ToJson (new EasyObjects (datas));
			}
		}
		return default (string);
	}

	static public T[] ConvertStringToArray<T> (string value)
	{
		Type type = typeof(T);
		T[] array = default (T[]);
		if (type == (typeof(string))) {
			array = JsonUtility.FromJson<EasyStrings> (value).ToList ().ToArray () as T[];
		} else if (type.IsSerializable && (type.IsClass || (type.IsValueType && !type.IsPrimitive))) {
			string[] arg = JsonUtility.FromJson<EasyStrings> (value).ToList ().ToArray ();
			array = new T[arg.Length];
			for (int i = 0; i < arg.Length; i++) {
				array [i] = JsonUtility.FromJson<T> (arg [i]);
			}
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsPrimitive) {
				EasyData[] datas = JsonUtility.FromJson<EasyObjects> (value).ToList ().ToArray ();
				array = new T[datas.Length];
				for (int i = 0; i < datas.Length; i++) {
					array [i] = (T)datas [i].GetObject ();
				}
			}
		}
		return array;
	}
}

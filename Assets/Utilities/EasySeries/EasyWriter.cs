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
		} else if (type.IsSerializable && (type.IsClass || type.IsSubclassOf (typeof(System.ValueType)))) {
			if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
				Debug.LogError ("Only plain classes and structures are supported; " +
				"classes derived from UnityEngine.Object (such as MonoBehaviour or ScriptableObject) are not." +
				"so please use Get<T> (string key, ref T target) replaced.");
			} else {
				return JsonUtility.FromJson<T> (GetObject (key).ToString ());
			}
		} else if (type.IsSerializable && type.IsArray) {
			Debug.LogError ("Sorry, we can not auto convert string to array type, " +
			"you can used Get<string> () function replaced, " +
			"then call EasyWriter.ConvertStringToArray<T> function convert string to target type.");
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsSubclassOf (typeof(System.ValueType))) {
			} else if (type.IsEnum) {
			} else if (type.IsSubclassOf (typeof(System.Nullable))) {
			} else {
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

	public void Set<T> (string key, T value)
	{
		Type type = typeof(T);
		if (type == (typeof(string))) {
			SetObject (key, value);
		} else if (type.IsSerializable && (type.IsClass || type.IsSubclassOf (typeof(System.ValueType)))) {
			#if UNITY_EDITOR
			SetObject (key, JsonUtility.ToJson (value, true));
			#else
			SetObject (key, JsonUtility.ToJson (value));
			#endif
		} else if (type.IsSerializable && type.IsArray) {
			string content = null;
			if (type == typeof(byte[]) || type == typeof(List<byte>))
				content = ConvertArrayToString<byte> (value);
			else if (type == typeof(bool[]) || type == typeof(List<bool>))
				content = ConvertArrayToString<bool> (value);
			else if (type == typeof(int[]) || type == typeof(List<int>))
				content = ConvertArrayToString<int> (value);
			else if (type == typeof(float[]) || type == typeof(List<float>))
				content = ConvertArrayToString<float> (value);
			else if (type == typeof(string[]) || type == typeof(List<string>))
				content = ConvertArrayToString<string> (value);
			else
				Debug.LogError ("Sorry, we can not auto convert array to string type, " +
				"you can used EasyWriter.ConvertArrayToString () function convert to string first, " +
				"then call this function to save data.");
			SetObject (key, content);
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsSubclassOf (typeof(System.ValueType))) {
			} else if (type.IsEnum) {
			} else if (type.IsSubclassOf (typeof(System.Nullable))) {
			} else {
				SetObject (key, value);
			}
		}
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
		} else if (type.IsSerializable && (type.IsClass || type.IsSubclassOf (typeof(System.ValueType)))) {
			string[] arg = new string[array.Length];
			for (int i = 0; i < arg.Length; i++) {
				arg [i] = JsonUtility.ToJson (array [i]);
			}
			return JsonUtility.ToJson (new EasyStrings (array as string[]));
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsSubclassOf (typeof(System.ValueType))) {
			} else if (type.IsEnum) {
			} else if (type.IsSubclassOf (typeof(System.Nullable))) {
			} else {
				return JsonUtility.ToJson (new EasyObjects (array as object[]));
			}
		}
		return default (string);
	}

	static public T[] ConvertStringToArray<T> (string value)
	{
		Type type = typeof(T);
		T[] array = default (T[]);
		if (type == (typeof(string))) {
			array = JsonUtility.FromJson<EasyStrings> (value).ToList () as T[];
		} else if (type.IsSerializable && (type.IsClass || type.IsSubclassOf (typeof(System.ValueType)))) {
			string[] arg = JsonUtility.FromJson<EasyStrings> (value).ToList ().ToArray ();
			array = new T[arg.Length];
			for (int i = 0; i < arg.Length; i++) {
				array [i] = JsonUtility.FromJson<T> (arg [i]);
			}
		} else if (type.IsSerializable && type.IsValueType) {
			if (type.IsSubclassOf (typeof(System.ValueType))) {
			} else if (type.IsEnum) {
			} else if (type.IsSubclassOf (typeof(System.Nullable))) {
			} else {
				array = JsonUtility.FromJson<EasyObjects> (value).ToList ().ToArray () as T[];
			}
		}
		return array;
	}
}

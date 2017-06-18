using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UniRx;

namespace UniEasy
{
	public partial class EasyWriter : IDisposable
	{
		private string filePath;
		private static Dictionary<string, EasyDictionary<string, EasyObject>> files;

		public EasyDictionary<string, EasyObject> target {
			get {
				return files [filePath];
			}
		}

		public EasyWriter (string path)
		{
			filePath = path;
			var directoryName = Path.GetDirectoryName (path);
			if (!Directory.Exists (directoryName)) {
				Directory.CreateDirectory (directoryName);
			}
			if (files == null) {
				files = new Dictionary<string, EasyDictionary<string, EasyObject>> ();
				if (Application.isPlaying)
					Observable.OnceApplicationQuit ().Subscribe (_ => {
						Dispose ();
					});
			}
			if (!files.ContainsKey (filePath)) {
				var value = Deserialize<EasyDictionary<string, EasyObject>> (filePath);
				if (value != null)
					files.Add (filePath, value);
				else
					files.Add (filePath, new EasyDictionary<string, EasyObject> (new Dictionary<string, EasyObject> ()));
			}
		}

		public bool HasKey (string key)
		{
			return target.ToDictionary ().ContainsKey (key);
		}

		public void Dispose ()
		{
			#if UNITY_EDITOR
			// If you create directly in the streamingassets folder,
			// The file created is not correct.
			if (filePath.Contains (Application.streamingAssetsPath)) {
				var fileName = Path.GetFileName (filePath);
				var tempPath = Application.dataPath + "/" + fileName;
				Serialize<EasyDictionary<string, EasyObject>> (tempPath, target);
				var newPath = filePath.Replace (Application.streamingAssetsPath, "Assets/StreamingAssets");
				UnityEditor.AssetDatabase.Refresh ();
				UnityEditor.AssetDatabase.DeleteAsset (newPath);
				UnityEditor.AssetDatabase.MoveAsset ("Assets/" + fileName, newPath);
			} else {
				Serialize<EasyDictionary<string, EasyObject>> (filePath, target);
			}
			UnityEditor.AssetDatabase.Refresh ();
			#else
			Serialize<EasyDictionary<string, EasyObject>> (filePath, target);
			#endif
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
			if (HasKey (key)) {
				target.ToDictionary () [key] = new EasyObject (value);
			} else {
				target.ToDictionary ().Add (key, new EasyObject (value));
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
				Debug.LogError ("Sorry, we can not auto convert string type to array type, " +
				"But you can use GetArray<T> (string key) method replaced.");
			} else if (type.IsSerializable && type.IsPrimitive) {
				return (T)GetObject (key);
			} else if (type.IsSerializable && type.IsEnum) {
			} else if (type.IsSerializable && type == typeof(Nullable)) {
			} else if (type.IsSerializable && (type.IsClass || type.IsValueType)) {
				if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
					Debug.LogError ("Sorry, we can not got the data whose is sub class UnityEngine.Object(such as MonoBehaviour or ScriptableObject) directly, " +
					"But you can use Get<T> (string key, T target) method replaced.");
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
				return content.ToArray<T> ();
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
				Debug.LogError ("Sorry, we can not auto convert array type to string type, " +
				"But you can use SetArray<T> (string key, object value) method replaced.");
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
				content = o.ToString<T> ();
			} else
				content = value.ToString<T> ();
			Set<string> (key, content);
		}

		public void Remove (string key)
		{
			var dictionary = target.ToDictionary ();
			if (dictionary != null && dictionary.ContainsKey (key)) {
				dictionary.Remove (key);
				files [filePath] = new EasyDictionary<string, EasyObject> (dictionary);
			}
		}

		public void Clear ()
		{
			files [filePath] = new EasyDictionary<string, EasyObject> (new Dictionary<string, EasyObject> ());
		}
	}
}

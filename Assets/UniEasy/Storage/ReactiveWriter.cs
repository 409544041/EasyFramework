using UniEasy.ECS;
using UnityEngine;
using System.IO;
using System;
using UniRx;

namespace UniEasy
{
	public class ReactiveWriter : IDisposableContainer
	{
		string filePath = "defaults";
		EasyDictionary<string, EasyObject> writer;

		private CompositeDisposable disposer = new CompositeDisposable ();

		public CompositeDisposable Disposer {
			get { return disposer; }
			set { disposer = value; }
		}

		public ReactiveWriter (string path, EasyDictionary<string, EasyObject> setter)
		{
			filePath = path;
			writer = setter;

			if (Application.isPlaying) {
				Observable.OnceApplicationQuit ().Subscribe (_ => {
					Dispose ();
				});
			}
		}

		public void Record ()
		{
			#if UNITY_EDITOR
			// You can not create assets(eg. json) directly in the streamingassets folder,
			// The assets be created have some issue can not be read,
			// So We need to create assets outside first,
			// Then move them to the streamingassets folder.
			if (filePath.Contains (Application.streamingAssetsPath)) {
				var fileName = Path.GetFileName (filePath);
				var tempPath = string.Format ("{0}/{1}", Application.dataPath, fileName);
				EasyWriter.Serialize<EasyDictionary<string, EasyObject>> (tempPath, writer);
				var newPath = filePath.Substring (filePath.IndexOf ("Assets"));
				UnityEditor.AssetDatabase.Refresh ();
				UnityEditor.AssetDatabase.DeleteAsset (newPath);
				UnityEditor.AssetDatabase.MoveAsset ("Assets/" + fileName, newPath);
				UnityEditor.AssetDatabase.Refresh ();
				return;
			}
			#endif
			EasyWriter.Serialize<EasyDictionary<string, EasyObject>> (filePath, writer);
		}

		public virtual void Dispose ()
		{
			Record ();
			Disposer.Dispose ();
		}

		public bool HasKey (string key)
		{
			return writer.ToDictionary ().ContainsKey (key);
		}

		public object GetObject (string key)
		{
			if (writer.ToDictionary ().ContainsKey (key)) {
				return writer.ToDictionary () [key].GetObject ();
			}
			return default (object);
		}

		public void SetObject (string key, object value)
		{
			if (HasKey (key)) {
				writer.ToDictionary () [key] = new EasyObject (value);
			} else {
				writer.ToDictionary ().Add (key, new EasyObject (value));
			}
			#if UNITY_EDITOR
			if (!Application.isPlaying) {
				Record ();
			}
			#endif
		}

		public T Get<T> (string key)
		{
			var type = typeof(T);
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
					Debug.LogError ("Sorry, we can not overwrite UnityEngine.Object Type(such as MonoBehaviour or ScriptableObject), " +
					"But you can use Get<T> (string key, T target) method replaced.");
				} else {
					return JsonUtility.FromJson<T> (GetObject (key).ToString ());
				}
			}
			return default (T);
		}

		public void Get<T> (string key, T target)
		{
			var type = target.GetType ();
			if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
				JsonUtility.FromJsonOverwrite (GetObject (key).ToString (), target);
			}
		}

		public void GetArray<T> (string key, T[] target)
		{
			var array = GetArray<string> (key);
			for (int i = 0; i < array.Length && i < target.Length; i++) {
				var type = target [i].GetType ();
				if (type.IsSubclassOf (typeof(UnityEngine.Object))) {
					JsonUtility.FromJsonOverwrite (array [i], target [i]);
				}
			}
		}

		public T[] GetArray<T> (string key)
		{
			var content = Get<string> (key);
			if (!string.IsNullOrEmpty (content)) {
				return content.ToArray<T> ();
			}
			return default (T[]);
		}

		public void Set<T> (string key, T value)
		{
			var type = typeof(T);
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
			var content = default (string);
			if (!value.GetType ().IsArray) {
				T[] o = new object[] { value } as T[];
				content = o.ToString<T> ();
			} else {
				content = value.ToString<T> ();
			}
			Set<string> (key, content);
		}

		public void Remove (string key)
		{
			var dic = writer.ToDictionary ();
			if (dic != null && dic.ContainsKey (key)) {
				dic.Remove (key);
				writer = new EasyDictionary<string, EasyObject> (dic);
			}
		}

		public void Clear ()
		{
			var dic = writer.ToDictionary ();
			if (dic != null) {
				dic.Clear ();
				writer = new EasyDictionary<string, EasyObject> (dic);
			}
		}
	}
}

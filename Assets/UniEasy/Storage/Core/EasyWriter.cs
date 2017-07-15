using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using UniRx;

namespace UniEasy
{
	public partial class EasyWriter
	{
		public static void Serialize<T> (string path, T t)
		{
			var fs = new FileStream (path, FileMode.Create);
			try {
				#if UNITY_EDITOR
				var serialize = JsonUtility.ToJson (t, true);
				#else
				var serialize = JsonUtility.ToJson (t);
				#endif
				var bytes = Encoding.UTF8.GetBytes (serialize);
				fs.Write (bytes, 0, bytes.Length);
			} catch (Exception e) {
				Debug.LogError ("Failed to serialize. Reason: " + e.Message);
				throw;
			} finally {
				fs.Close ();
			}
		}

		public static T Deserialize<T> (string path)
		{
			T t = default (T);
			if (File.Exists (path)) {
				var fs = new FileStream (path, FileMode.Open);
				try {
					var bytes = new byte[(int)fs.Length];
					fs.Read (bytes, 0, bytes.Length);
					var value = Encoding.UTF8.GetString (bytes);
					t = JsonUtility.FromJson<T> (value);
				} catch (Exception e) {
					Debug.LogError ("Failed to deserialize. Reason: " + e.Message);
					throw;
				} finally {
					fs.Close ();
				}
			}
			return t;
		}

		public static UniRx.IObservable<T> DeserializeAsync<T> (string path)
		{
			#if UNITY_ANDROID
			if (path.StartsWith (Application.streamingAssetsPath)) {
				#if UNITY_EDITOR
				path = "file://" + path;
				#endif
				return Observable.FromCoroutine<string> ((observer, cancellationToken) => {
					return GetWWWCore (path, observer, cancellationToken);
				}).Last ().Select (x => JsonUtility.FromJson<T> (x));
			}
			#endif
			return Observable.ToObservable<T> (new T[] { Deserialize<T> (path) });
		}

		public static IEnumerator GetWWWCore (string url, UniRx.IObserver<string> observer, CancellationToken cancellationToken)
		{
			var www = new WWW (url);
			while (!www.isDone && !cancellationToken.IsCancellationRequested) {
				yield return null;
			}

			if (cancellationToken.IsCancellationRequested) {
				yield break;
			}

			if (!string.IsNullOrEmpty (www.error)) {
				observer.OnError (new Exception (www.error));
			} else {
				observer.OnNext (www.text);
				observer.OnCompleted ();
			}
		}

		public static byte[] SerializeObject (object obj)
		{
			if (obj == null) {
				return null;
			}
			var ms = new MemoryStream ();
			var formatter = new BinaryFormatter ();
			formatter.Serialize (ms, obj);
			ms.Position = 0;
			var bytes = ms.GetBuffer ();
			ms.Read (bytes, 0, bytes.Length);
			ms.Close ();
			return bytes;
		}

		public static object DeserializeObject (byte[] bytes)
		{
			object obj = null;
			if (bytes == null || bytes.Length <= 0) {
				return obj;
			}
			var ms = new MemoryStream (bytes);
			ms.Position = 0;
			var formatter = new BinaryFormatter ();
			obj = formatter.Deserialize (ms);
			ms.Close ();
			return obj;
		}
	}
}

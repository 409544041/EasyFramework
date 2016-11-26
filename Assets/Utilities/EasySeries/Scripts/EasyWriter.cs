using UnityEngine;
using System.Text;
using System.IO;
using System;

// create by chaolun 2016/11/26
public class EasyWriter
{
	static public void Serialize<T> (string path, T t)
	{
		FileStream fs = new FileStream (path, FileMode.OpenOrCreate);
		try {
			#if UNITY_EDITOR
			string serialize = JsonUtility.ToJson (t, true);
			#else
			string serialize = JsonUtility.ToJson (t);
			#endif
			byte[] bytes = Encoding.UTF8.GetBytes (serialize);
			fs.Write (bytes, 0, bytes.Length);
		} catch (Exception e) {
			Debug.LogError ("Failed to serialize. Reason: " + e.Message);
			throw;
		} finally {
			fs.Close ();
		}
	}

	static public T Deserialize<T> (string path)
	{
		T t = default (T);
		if (File.Exists (path)) {
			FileStream fs = new FileStream (path, FileMode.Open);
			try {
				byte[] bytes = new byte[(int)fs.Length];
				fs.Read (bytes, 0, bytes.Length);
				string value = Encoding.UTF8.GetString (bytes);
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
}

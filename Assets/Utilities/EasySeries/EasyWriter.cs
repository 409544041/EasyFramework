using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public partial class EasyWriter
{
	static public void Serialize<T> (string path, T t)
	{
		FileStream fs = new FileStream (path, FileMode.Create);
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

	public static byte[] SerializeObject (object obj)
	{
		if (obj == null) {
			return null;
		}
		MemoryStream ms = new MemoryStream ();
		BinaryFormatter formatter = new BinaryFormatter ();
		formatter.Serialize (ms, obj);
		ms.Position = 0;
		byte[] bytes = ms.GetBuffer ();
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
		MemoryStream ms = new MemoryStream (bytes);
		ms.Position = 0;
		BinaryFormatter formatter = new BinaryFormatter ();
		obj = formatter.Deserialize (ms);
		ms.Close ();
		return obj;
	}
}

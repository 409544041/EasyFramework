using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

// create by chaolun 2016/11/26
public class EasyWriter
{

	static public void Serialize (string path, Hashtable hashtable)
	{
		Hashtable hs = Deserialize (path);
		if (hashtable != null && hs != null) {
			IEnumerator enumerator = hs.Keys.GetEnumerator ();
			while (enumerator.MoveNext ()) {
				if (!hashtable.ContainsKey (enumerator.Current)) {
					hashtable.Add (enumerator.Current, hs [enumerator.Current]);
				}
			}
		}
		FileStream fs = new FileStream (path, FileMode.OpenOrCreate);
		BinaryFormatter formatter = new BinaryFormatter ();
		try {
			formatter.Serialize (fs, hashtable);
		} catch (SerializationException e) {
			Debug.LogError ("Failed to serialize. Reason: " + e.Message);
			throw;
		} finally {
			fs.Close ();
		}
	}

	static public Hashtable Deserialize (string path)
	{
		Hashtable hashtable = null;
		if (File.Exists (path)) {
			FileStream fs = new FileStream (path, FileMode.Open);
			try {
				BinaryFormatter formatter = new BinaryFormatter ();
				hashtable = (Hashtable)formatter.Deserialize (fs);
			} catch (SerializationException e) {
				Debug.LogError ("Failed to deserialize. Reason: " + e.Message);
				throw;
			} finally {
				fs.Close ();
			}
		}
		return hashtable;
	}
}

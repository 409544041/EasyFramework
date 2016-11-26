using UnityEngine;
using System.Collections;
using System.IO;

// create by chaolun 2016/11/26
public class EasyWriterExample : MonoBehaviour
{

	void Start ()
	{
		Hashtable hs = new Hashtable ();
		hs.Add ("byte", 0x01010101);
		hs.Add ("int", 9876543210);
		hs.Add ("float", 0.123456789f);
		EasyWriter.Serialize (Application.persistentDataPath + "/example.json", hs);
//		string path = Path.GetFullPath (Application.persistentDataPath);
//		System.Diagnostics.Process.Start ("explorer.exe", path);

		Hashtable hashtable = EasyWriter.Deserialize (Application.persistentDataPath + "/example.json");
		IDictionaryEnumerator de = hashtable.GetEnumerator ();
		while (de.MoveNext ()) {
			Debug.Log (de.Key + " : " + de.Value);
		}
	}
}

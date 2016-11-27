using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

// create by chaolun 2016/11/26
public class EasyWriterExample : MonoBehaviour
{

	void Start ()
	{
		Dictionary<string, EasyByte> dic = new Dictionary<string, EasyByte> ();
		dic.Add ("bool", new EasyByte (EasyWriter.SerializeObject (false)));
		dic.Add ("int", new EasyByte (EasyWriter.SerializeObject (9876543210)));
		dic.Add ("float", new EasyByte (EasyWriter.SerializeObject (0.123456789f)));
		dic.Add ("string", new EasyByte (EasyWriter.SerializeObject ("chao-lun:examlpe!?")));
		EasyBasic<string, EasyByte> ej = new EasyBasic<string, EasyByte> (dic);
		EasyWriter.Serialize (Application.persistentDataPath + "/example.json", ej);
//		string path = Path.GetFullPath (Application.persistentDataPath);
//		System.Diagnostics.Process.Start ("explorer.exe", path);

		EasyBasic<string, EasyByte> easyJson = EasyWriter.Deserialize<EasyBasic<string, EasyByte>> (Application.persistentDataPath + "/example.json");
		dic = easyJson.ToDictionary ();
		Dictionary<string, EasyByte>.KeyCollection.Enumerator e = dic.Keys.GetEnumerator ();
		while (e.MoveNext ()) {
			object o = EasyWriter.DeserializeObject (dic [e.Current].ToList ().ToArray ());
			Debug.Log (e.Current + " : " + o);
		}
	}
}

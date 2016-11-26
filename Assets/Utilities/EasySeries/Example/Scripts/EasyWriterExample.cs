using UnityEngine;
using System.Collections.Generic;
using System.IO;

// create by chaolun 2016/11/26
public class EasyWriterExample : MonoBehaviour
{

	void Start ()
	{
		Dictionary<string, string> dic = new Dictionary<string, string> ();
		dic.Add ("0", "qwertyuiopasdfghjklzxcvbnm");
		EasyJson<string, string> ej = new EasyJson<string, string> (dic);
		EasyWriter.Serialize (Application.persistentDataPath + "/example.json", ej);
//		string path = Path.GetFullPath (Application.persistentDataPath);
//		System.Diagnostics.Process.Start ("explorer.exe", path);

		EasyJson<string, string> easyJson = EasyWriter.Deserialize<EasyJson<string, string>> (Application.persistentDataPath + "/example.json");
		Debug.Log (easyJson.ToDictionary () ["0"]);
	}
}

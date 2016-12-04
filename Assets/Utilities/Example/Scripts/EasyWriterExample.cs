using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

// create by chaolun 2016/11/26
public class EasyWriterExample : MonoBehaviour
{
	[SerializeField]
	private GameObject gameObject;
	[SerializeField]
	private EasyBlock[] blocks;

	void Start ()
	{
		Dictionary<string, EasyData> dic = new Dictionary<string, EasyData> ();
		dic.Add ("empty", new EasyData ());
		dic.Add ("bool", new EasyData (false));
		dic.Add ("int", new EasyData (123456789));
		dic.Add ("float", new EasyData (0.123456789f));
		dic.Add ("string", new EasyData ("cool, now we can save different datas in a json file!"));
		EasyData<string, EasyData> easyValue = new EasyData<string, EasyData> (dic);
		EasyWriter.Serialize (Application.persistentDataPath + "/example.json", easyValue);

//		string path = Path.GetFullPath (Application.persistentDataPath);
//		System.Diagnostics.Process.Start ("explorer.exe", path);

		EasyData<string, EasyData> easyJson = EasyWriter.Deserialize<EasyData<string, EasyData>> (Application.persistentDataPath + "/example.json");
		dic = easyJson.ToDictionary ();
//		Dictionary<string, EasyData>.KeyCollection.Enumerator e = dic.Keys.GetEnumerator ();
//		while (e.MoveNext ()) {
//			int o = dic [e.Current].GetObject<int> ();
//			Debug.Log (e.Current + " : " + o);
//		}
		bool o = (bool)dic ["bool"].GetObject ();
		Debug.Log ("bool" + " : " + o);

		List<string> list = new List<string> ();
		for (int i = 0; i < blocks.Length; i++) {
			BlockFactory.Instance.AddBlock (blocks [i]);
			list.Add (blocks [i].name);
		}
		BlockFactory.Instance.CreateSnapShot ("cubes", list);
		BlockFactory.Instance.CreateBlockBySnapShots ("cubes");
	}
}

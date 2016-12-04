using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

// create by chaolun 2016/11/26
public class EasyWriterExample : MonoBehaviour
{
	[SerializeField]
	private GameObject go;
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

		EasyPrefs prefs = new EasyPrefs (Application.persistentDataPath + "/example.json");
		Debug.Log (prefs.GetObject ("empty"));
		Debug.Log (prefs.GetObject ("bool"));
		Debug.Log (prefs.GetObject ("int"));
		Debug.Log (prefs.GetObject ("float"));
		Debug.Log (prefs.GetObject ("string"));

		prefs.SetObject ("empty", "");
		prefs.SetObject ("bool", true);
		prefs.SetObject ("int", -123456789);
		prefs.SetObject ("float", -0.123456789f);
		prefs.SetObject ("string", "酷，现在我们可以在一个文件中储存各种类型的数据了！");

		Debug.Log (prefs.GetObject ("empty"));
		Debug.Log (prefs.GetObject ("bool"));
		Debug.Log (prefs.GetObject ("int"));
		Debug.Log (prefs.GetObject ("float"));
		Debug.Log (prefs.GetObject ("string"));

		List<string> list = new List<string> ();
		for (int i = 0; i < blocks.Length; i++) {
			BlockFactory.Instance.AddBlock (blocks [i]);
			list.Add (blocks [i].name);
		}
		BlockFactory.Instance.CreateSnapShot ("cubes", list);
		BlockFactory.Instance.CreateBlockBySnapShots ("cubes");
	}
}

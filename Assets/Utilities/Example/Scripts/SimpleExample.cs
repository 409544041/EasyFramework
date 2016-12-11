using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class SimpleExample : MonoBehaviour
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

		EasyWriter writer = new EasyWriter (Application.persistentDataPath + "/example.json");
		Debug.Log (writer.GetObject ("empty"));
		Debug.Log (writer.GetObject ("bool"));
		Debug.Log (writer.GetObject ("int"));
		Debug.Log (writer.GetObject ("float"));
		Debug.Log (writer.GetObject ("string"));

		writer.SetObject ("empty", "");
		writer.SetObject ("bool", true);
		writer.SetObject ("int", -123456789);
		writer.SetObject ("float", -0.123456789f);
		writer.SetObject ("string", "酷，现在我们可以在一个文件中储存各种类型的数据了！");

//		File.WriteAllText (Application.persistentDataPath + "/asset.bytes", JsonUtility.ToJson (blocks [0]));

		Debug.Log (writer.GetObject ("empty"));
		Debug.Log (writer.GetObject ("bool"));
		Debug.Log (writer.GetObject ("int"));
		Debug.Log (writer.GetObject ("float"));
		Debug.Log (writer.GetObject ("string"));

		List<string> list = new List<string> ();
		for (int i = 0; i < blocks.Length; i++) {
			BlockFactory.Instance.AddBlock (blocks [i]);
			list.Add (blocks [i].name);
		}
		BlockFactory.Instance.CreateSnapShot ("cubes", list);
		BlockFactory.Instance.CreateBlockBySnapShots ("cubes");
	}

	IEnumerator a (string path)
	{
		WWW www = new WWW (path);
		while (!www.isDone) {
			yield return null;
		}
		Debug.Log ("www" + www.bytes.ToString ());
	}
}

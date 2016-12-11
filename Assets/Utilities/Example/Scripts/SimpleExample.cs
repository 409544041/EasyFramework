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
		EasyWriter writer = new EasyWriter (Application.persistentDataPath + "/example.json");

		writer.Set<byte> ("byte", byte.MaxValue);
		writer.Set<bool> ("bool", true);
		writer.Set<int> ("int", -123456789);
		writer.Set<float> ("float", -0.123456789f);
		writer.Set<string> ("string", "酷，现在我们可以在一个文件中储存各种类型的数据了！");

		Debug.Log (writer.Get<byte> ("byte"));
		Debug.Log (writer.Get<bool> ("bool"));
		Debug.Log (writer.Get<int> ("int"));
		Debug.Log (writer.Get<float> ("float"));
		Debug.Log (writer.Get<string> ("string"));

//		string path = Path.GetFullPath (Application.persistentDataPath);
//		System.Diagnostics.Process.Start ("explorer.exe", path);

		List<string> list = new List<string> ();
		for (int i = 0; i < blocks.Length; i++) {
			BlockFactory.Instance.AddBlock (blocks [i]);
			list.Add (blocks [i].name);
		}
		BlockFactory.Instance.CreateSnapShot ("cubes", list);
		BlockFactory.Instance.CreateBlockBySnapShots ("cubes");
	}
}

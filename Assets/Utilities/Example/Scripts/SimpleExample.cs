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

	[System.Serializable]
	public struct SimpleData
	{
		public Vector3 position;
	}

	void Start ()
	{
		EasyWriter writer = new EasyWriter (Application.persistentDataPath + "/example.json");

		writer.Set<byte> ("byte", byte.MaxValue);
		writer.Set<bool> ("bool", true);
		writer.Set<int> ("int", -123456789);
		writer.Set<float> ("float", -0.123456789f);
		writer.Set<string> ("string", "酷，现在我们可以在一个文件中储存各种类型的数据了！");
		writer.SetArray<byte> ("byte[]", new byte[] { byte.MinValue, byte.MaxValue });
		writer.SetArray<bool> ("bool[]", new bool[] { true, false, true, false, false });
		writer.SetArray<int> ("int[]", new int[] { -1, -2, -3, 4, 5, 6, 7, -8, -9 });
		writer.SetArray<float> ("float[]", new float[] { -0.12f, -34.56f, 7.89f, 1.114f });
		writer.SetArray<string> ("string[]", new string[] { "a0", "b1", "c2", "d3", "e4", "f5" });
		writer.Set<SimpleData> ("simple data", new SimpleData () { position = new Vector3 (100, 120, -200) });
		writer.SetArray<SimpleData> ("simple data array", new SimpleData [] {
			new SimpleData { position = new Vector3 (-520, 950, -730) },
			new SimpleData { position = new Vector3 (725, -942, 146) }
		});

		Debug.Log (writer.Get<byte> ("byte"));
		Debug.Log (writer.Get<bool> ("bool"));
		Debug.Log (writer.Get<int> ("int"));   
		Debug.Log (writer.Get<float> ("float"));
		Debug.Log (writer.Get<string> ("string"));
		Debug.Log (writer.GetArray<byte> ("byte[]") [0]);
		Debug.Log (writer.GetArray<bool> ("bool[]") [3]);
		Debug.Log (writer.GetArray<int> ("int[]") [4]);
		Debug.Log (writer.GetArray<float> ("float[]") [3]);
		Debug.Log (writer.GetArray<string> ("string[]") [5]);
		Debug.Log (writer.Get<SimpleData> ("simple data").position);
		Debug.Log (writer.GetArray<SimpleData> ("simple data array") [0].position);

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

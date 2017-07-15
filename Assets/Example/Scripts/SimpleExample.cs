using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UniEasy;
using UniEasy.Console;
using UniRx;

[System.Serializable]
public class SimpleExample : MonoBehaviour
{
	[SerializeField]
	private bool isOpenFile;
	[SerializeField]
	private GameObject go;
	[SerializeField]
	private EasyBlock[] blocks;

	[System.Serializable]
	public struct StructSample
	{
		public Vector3 position;
	}

	[System.Serializable]
	public class ClassSample
	{
		public Rect rect;
	}

	void Start ()
	{
		EasyWriter writer = new EasyWriter (Application.persistentDataPath + "/example.json");

		writer.OnAdd ().Subscribe (x => {
			x.Set<byte> ("byte", byte.MaxValue);
			x.Set<bool> ("bool", true);
			x.Set<int> ("int", -123456789);
			x.Set<float> ("float", -0.123456789f);
			x.Set<string> ("string", "Test 'EasyWriter' function :P");
			x.SetArray<byte> ("byte[]", new byte[] { byte.MinValue, byte.MaxValue });
			x.SetArray<bool> ("bool[]", new bool[] { true, false, true, false, false });
			x.SetArray<int> ("int[]", new int[] { -1, -2, -3, 4, 5, 6, 7, -8, -9 });
			x.SetArray<float> ("float[]", new float[] { -0.12f, -34.56f, 7.89f, 1.114f });
			x.SetArray<string> ("string[]", new string[] { "a0", "b1", "c2", "d3", "e4", "f5" });
			x.Set<StructSample> ("struct", new StructSample () { position = new Vector3 (100, 120, -200) });
			x.SetArray<StructSample> ("struct array", new StructSample [] {
				new StructSample { position = new Vector3 (-520, 950, -730) },
				new StructSample { position = new Vector3 (725, -942, 146) }
			});
			x.Set<ClassSample> ("class", new ClassSample () { rect = new Rect (10, 20, 30, 40) });
			x.SetArray<ClassSample> ("class array", new ClassSample [] {
				new ClassSample () { rect = new Rect (120, -145, 274, -368) },
				new ClassSample () { rect = new Rect (-150, 160, -170, 180) }
			});
			x.Set<SimpleExample> ("this", this);
			x.SetArray<SimpleExample> ("this array", new SimpleExample [] {
				this,
				this
			});
			x.Set<EasyBlock> ("block", blocks [0]);
			x.SetArray<EasyBlock> ("block array", blocks);

			Debugger.Log (x.Get<byte> ("byte"));
			Debugger.Log (x.Get<bool> ("bool"));
			Debugger.Log (x.Get<int> ("int"));   
			Debugger.Log (x.Get<float> ("float"));
			Debugger.Log (x.Get<string> ("string"));
			Debugger.Log (x.GetArray<byte> ("byte[]") [0]);
			Debugger.Log (x.GetArray<bool> ("bool[]") [3]);
			Debugger.Log (x.GetArray<int> ("int[]") [4]);
			Debugger.Log (x.GetArray<float> ("float[]") [3]);
			Debugger.Log (x.GetArray<string> ("string[]") [5]);
			Debugger.Log (x.Get<StructSample> ("struct").position);
			Debugger.Log (x.GetArray<StructSample> ("struct array") [0].position);
			Debugger.Log (x.Get<ClassSample> ("class").rect);
			Debugger.Log (x.GetArray<ClassSample> ("class array") [1].rect);
			x.Get<SimpleExample> ("this", this);
			x.GetArray<SimpleExample> ("this array", new SimpleExample[] { this, this });
			x.Get<EasyBlock> ("block", blocks [0]);
			x.GetArray<EasyBlock> ("block array", blocks);

			x.Remove ("string");
			Debugger.Log ("already remove 'string' key : " + x.Get<string> ("string"));
			x.Clear ();
			Debugger.Log ("already clear : " + x.Get<string> ("string[]"));
		});

		if (isOpenFile) {
			string path = Path.GetFullPath (Application.persistentDataPath);
			System.Diagnostics.Process.Start ("explorer.exe", path);
		}

		List<string> list = new List<string> ();
		for (int i = 0; i < blocks.Length; i++) {
			BlockFactory.Instance.AddBlock (blocks [i]);
			list.Add (blocks [i].name);
		}
		BlockFactory.Instance.CreateSnapShot ("cubes", list);
		BlockFactory.Instance.CreateBlockBySnapShots ("cubes");
	}
}

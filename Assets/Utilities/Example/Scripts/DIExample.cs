using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UniEasy;

public class DIExample : MonoBehaviour
{
	[EasySingle]
	public GameObjectFactory factory;
	[EasySingle]
	public DITest test;
	public bool isOn;

	void Start ()
	{
		Debug.Log (factory);
		Debug.Log (test);

		if (isOn) {
			GameObject go = new GameObject ("DI Test");
			go.AddComponent<DIExample> ();
		}

		var fieldInfos = typeof(GameObjectFactory).GetAllInstanceFields ()
			.Where (x => x.HasAttribute (typeof(EasySingleAttribute))).ToList ();
		for (int n = 0; n < fieldInfos.Count; n++) {
			Debug.Log ("xxxxx" + fieldInfos [n]);
		}
	}
}

public class DITest
{
	
}

using UnityEngine;
using System.Collections;
using System;

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
	}
}

public class DITest
{
	
}

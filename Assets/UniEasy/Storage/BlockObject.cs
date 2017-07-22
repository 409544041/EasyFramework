using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable] 
	public class BlockObject
	{
		public string blockName;
		public GameObject gameObject;
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;

		public BlockObject (string name)
		{
			blockName = name;
		}
	}
}

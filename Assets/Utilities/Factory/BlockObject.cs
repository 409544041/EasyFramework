using UnityEngine;
using System;

namespace UniEasy
{
	[Serializable] 
	public class BlockObject
	{
		public string name;
		public GameObject gameObject;
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;

		public BlockObject (string name)
		{
			this.name = name;
		}
	}
}

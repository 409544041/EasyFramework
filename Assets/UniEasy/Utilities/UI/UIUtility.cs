using UnityEngine.UI;
using UnityEngine;

namespace UniEasy
{
	public static class UIUtility
	{
		public static T Create<T> (string name, Transform parent = null) where T : Component
		{
			var go = new GameObject (name);
			go.transform.SetParent (parent);
			go.layer = LayerMask.NameToLayer ("UI");
			return go.AddComponent<T> ();
		}
	}
}
